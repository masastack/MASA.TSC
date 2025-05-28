// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using GraphQL.Client.Http;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
using Masa.Tsc.Web.Admin.Rcl.Cubejs;
using Masa.Tsc.Web.Admin.Rcl.Cubejs.Request;
using Masa.Tsc.Web.Admin.Rcl.Cubejs.Response;
using System.Collections.Generic;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Endpoint
{

    const double MILLSECOND = 1e6;

    [Parameter]
    public string TraceId { get; set; }

    [Inject(Key = RclServiceCollectionExtensions.Cubejs_Client_Name)]
    GraphQL.Client.Http.GraphQLHttpClient Client { get; set; }

    protected override bool IsPage => true;
    protected override bool IsEndPointPage => true;
    private List<DataTableHeader<ListChartData>> headers => new()
    {
        new() { Text = I18n.Apm("Endpoint.List.Name"), Value = nameof(ListChartData.Name) },
        new() { Text = I18n.Apm("Endpoint.List.Service"), Value =nameof(ListChartData.Service)},
        new() { Text = I18n.Apm("Endpoint.List.Latency"), Value = nameof(ListChartData.Latency)},
        new() { Text = I18n.Apm("Endpoint.List.Throughput"), Value = nameof(ListChartData.Throughput)},
        new() { Text = I18n.Apm("Endpoint.List.Failed"), Value = nameof(ListChartData.Failed)}
    };

    private int defaultSize = 50;
    private int total = 0;
    private int page = 1;
    private List<ListChartData> data = new();
    private bool isTableLoading = false;
    private string? sortFiled;
    private bool? sortBy;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (!string.IsNullOrEmpty(TraceId))
        {
            Search.TraceId = TraceId;
        }
    }

    public async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy[0];
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc[0];
        else
            sortBy = default;
        await LoadASync();
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadASync();
    }

    private async Task LoadASync(SearchData data = null!)
    {
        if (data != null)
        {
            page = 1;
            Search = data;
        }
        if (Search.Start == DateTime.MinValue || Search.End == DateTime.MinValue)
            return;
        isTableLoading = true;
        StateHasChanged();
        //await LoadPageDataAsync();
        await LoadCubePageDataAsync();
        isTableLoading = false;
        StateHasChanged();
        await LoadChartDataAsync();
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            OrderField = sortFiled,
            Env = Search.Environment,
            IsDesc = sortBy,
            Service = Search.Service,
            TraceId = Search.TraceId,
            ExType = Search.ExceptionType,
            StatusCodes = Search.Status,
            TextField = Search.TextField,
            TextValue = Search.TextValue,
            Endpoint = Search.Endpoint ?? string.Empty,
            StatusCode = Search.Status,
            //Queries = Search.Text
        };
        var result = await ApiCaller.ApmService.GetEndpointPageAsync(CurrentTeamId, query, Search.Project, Search.ServiceType);
        data.Clear();
        total = 0;
        if (result != null && result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result.Select(item => new ListChartData
            {
                Name = $"{item.Method} {item.Endpoint}",
                Method = item.Method,
                Endpoint = item.Endpoint,
                Service = item.Service,
                Failed = item.Failed,
                Throughput = item.Throughput,
                Latency = item.Latency
            }));
            total = (int)result.Total;
        }
    }


    private async Task LoadCubePageDataAsync()
    {
        var where = CubeJsRequestUtils.GetEndpintListWhere(Search.Start, Search.End, Search.Environment, Search.Service, CurrentTeamId, Search.Endpoint, Search.Method);
        var totalRequest = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_LIST_VIEW, where, fields: CubejsConstants.ENDPOINT_LIST_COUNT));
        isTableLoading = true;
        var totalResponse = await Client.SendQueryAsync<CubejsBaseResponse<EndpointTotalResponse>>(totalRequest);
        total = (int)totalResponse.Data.Data[0].Item.Total;
        data.Clear();
        if (total == 0)
        {
            isTableLoading = true;
            return;
        }

        var pageRequest = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_LIST_VIEW, where, default, page, defaultSize, CubejsConstants.SERVICENAME, CubejsConstants.TARGET, CubejsConstants.METHOD, CubejsConstants.FAILED_AGG, CubejsConstants.LATENCY_AGG, CubejsConstants.THROUGHPUT));
        var pageResponse = await Client.SendQueryAsync<CubejsBaseResponse<EndpointListResponse>>(pageRequest);

        var totalMinits = Math.Floor((Search.End - Search.Start).TotalMinutes);
        if (pageResponse != null && pageResponse.Data.Data != null && pageResponse.Data.Data.Count > 0)
        {
            data.AddRange(pageResponse.Data.Data.Select(item => new ListChartData
            {
                Name = $"{item.Item.Method} {item.Item.Target}",
                Method = item.Item.Method,
                Endpoint = item.Item.Target,
                Service = item.Item.ServiceName,
                Failed = Math.Round(item.Item.FailedAgg * 100.0, 2),
                Throughput = Math.Round(item.Item.Throughput / totalMinits, 3),
                Latency = (long)Math.Floor(item.Item.LatencyAgg / MILLSECOND),
            }));
        }
        isTableLoading = true;
    }

    private async Task LoadChartDataAsync()
    {
        if (data.Count == 0)
            return;
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            Endpoint = Search.Endpoint!,
            Service = Search.Service,
            Env = Search.Environment
        };
        var result = await ApiCaller.ApmService.GetChartsAsync(query);
        if (result == null || result.Count == 0)
        {
            return;
        }

        foreach (var service in data)
        {
            var chartData = result.Find(s => s.Name == service.Name);
            service.LatencyChartData = new();
            service.ThroughputChartData = new();
            service.FailedChartData = new();
            if (chartData == null)
            {
                service.LatencyChartData.EmptyChart = true;
                service.ThroughputChartData.EmptyChart = true;
                service.FailedChartData.EmptyChart = true;
                var emptyObj = new object();
                service.LatencyChartData.Data = emptyObj;
                service.ThroughputChartData.Data = emptyObj;
                service.FailedChartData.Data = emptyObj;
            }
            else
            {
                service.LatencyChartData.Data = ConvertLatencyChartData(chartData, item => item.Latency).Json;
                service.ThroughputChartData.Data = ConvertLatencyChartData(chartData, item => item.Throughput).Json;
                service.FailedChartData.Data = ConvertLatencyChartData(chartData, item => item.Failed).Json;
            }
            service.LatencyChartData.ChartLoading = false;
            service.ThroughputChartData.ChartLoading = false;
            service.FailedChartData.ChartLoading = false;
        }
        StateHasChanged();
    }

    private static EChartType ConvertLatencyChartData(ChartLineDto data, Func<ChartLineItemDto, object> fnProperty, string lineColor = null, string areaLineColor = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { });
        chart.SetValue("legend", new { });
        chart.SetValue("xAxis.show", false);
        chart.SetValue("yAxis.show", false);
        chart.SetValue("grid", new { top = "0%", left = "0%", right = "0%", bottom = "0%" });
        var index = 0;
        if (data.Currents != null && data.Currents.Any())
        {
            chart.SetValue($"series[{index++}]", new { type = "line", smooth = true, symbol = "none", data = data.Currents.Select(fnProperty) });
        }
        if (data.Previous != null && data.Previous.Any())
        {
            chart.SetValue($"series[{index}]", new { type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data.Previous.Select(fnProperty) });
        }

        return chart;
    }
}
