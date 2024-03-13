// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Endpoint
{
    protected override bool IsPage => true;
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

    public async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy.First();
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc.First();
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
        isTableLoading = true;
        if (data != null)
            Search = data;
        StateHasChanged();
        await LoadPageDataAsync();
        isTableLoading = false;
        StateHasChanged();
        await LoadChartDataAsync();
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new BaseApmRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            OrderField = sortFiled,
            Env = Search.Enviroment,
            IsDesc = sortBy,
            Service = Search.Service,
            Queries=Search.Text
        };
        var result = await ApiCaller.ApmService.GetEndpointPageAsync(query);
        data.Clear();
        if (result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result.Select(item => new ListChartData
            {
                Name = $"{item.Method} {item.Name}",
                Endpoint = item.Name,
                Service = item.Service,
                Failed = item.Failed,
                Throughput = item.Throughput,
                Latency = item.Latency
            }));
        }
        total = (int)result.Total;
    }

    private async Task LoadChartDataAsync()
    {
        if (data.Count == 0)
            return;
        var query = new BaseApmRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            Service = Search.Service,
            Env = Search.Enviroment
        };
        var result = await ApiCaller.ApmService.GetChartsAsync(query);
        if (result == null || !result.Any())
        {
            return;
        }

        foreach (var service in data)
        {
            var chartData = result.FirstOrDefault(s => s.Name == service.Name);
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
