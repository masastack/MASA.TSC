// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Endpoint
{
    [Parameter]
    public string TraceId { get; set; }

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
        await LoadCubeChartDataAsync();
        isTableLoading = false;
        StateHasChanged();
        //await LoadChartDataAsync();
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

    private async Task LoadCubePageDataAsync()
    {
        var teamId = Guid.Parse("77ad20db-729f-4120-bf9c-6978f2d0ec2c");
        var where = CubeJsRequestUtils.GetEndpintListWhere(Search.Start, Search.End, teamId, Search.Environment, Search.Service, Search.Endpoint, Search.Method, Search.Project);
        var totalRequest = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_LIST_VIEW, where, fields: CubejsConstants.ENDPOINT_LIST_COUNT));
        isTableLoading = true;
        Console.WriteLine("LoadCubePageDataAsync page start,{0}", DateTime.Now);
        var totalResponse = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointTotalResponse>>(totalRequest);
        Console.WriteLine("LoadCubePageDataAsync page end,{0}", DateTime.Now);
        total = (int)totalResponse.Data.Data[0].Item.Total;

        data.Clear();
        if (total == 0)
        {
            return;
        }

        var orderBy = CubeJsRequestUtils.GetEndpintListOrderBy(sortFiled, sortBy);
        var pageRequest = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_LIST_VIEW, where, orderBy, page, defaultSize, CubejsConstants.SERVICENAME, CubejsConstants.TARGET, CubejsConstants.METHOD, CubejsConstants.FAILED_AGG, CubejsConstants.LATENCY_AGG, CubejsConstants.THROUGHPUT));
        Console.WriteLine("LoadCubePageDataAsync pagedata start,{0}", DateTime.Now);
        var pageResponse = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointListResponse>>(pageRequest);
        Console.WriteLine("LoadCubePageDataAsync pagedata end,{0}", DateTime.Now);

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
    }

    private async Task LoadCubeChartDataAsync()
    {
        isTableLoading = true;
        if (data.Count == 0)
            return;
        var teamId = Guid.Parse("77ad20db-729f-4120-bf9c-6978f2d0ec2c");
        var services = data.Select(item => item.Service).Distinct().ToArray();
        var targets = data.Select(item => item.Name.Split(' ')[1]).Distinct().ToArray();
        var methods = data.Select(item => item.Name.Split(' ')[0]).Distinct().ToArray();
        var result = new List<ChartLineDto>();
        Console.WriteLine("LoadCubePageDataAsync chartdata1 start,{0}", DateTime.Now);
        var list = await GetChartDataAsync(Search.Start, Search.End, teamId, services, targets, methods);
        Console.WriteLine("LoadCubePageDataAsync pagedata1 end,{0}", DateTime.Now);


        SetChartData(result, list, false, false);
        (bool hasPrious, DateTime start, DateTime end) = SetAndCheckPreviousTime();
        if (hasPrious)
        {
            Console.WriteLine("LoadCubePageDataAsync chartdata2 start,{0}", DateTime.Now);
            var previousList = await GetChartDataAsync(start, end, teamId, services, targets, methods);
            Console.WriteLine("LoadCubePageDataAsync pagedata2 end,{0}", DateTime.Now);
            SetChartData(result, list, false, true);
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


    private async Task<List<EndpointListChartItemResponse>> GetChartDataAsync(DateTime start, DateTime end, Guid teamId, string[] services, string[] endpoints, string[] methods)
    {
        var where = CubeJsRequestUtils.GetEndpintListChartWhere(start, end, Search.Environment, teamId, services, endpoints, methods);
        var orderBy = $"{CubejsConstants.SERVICENAME}:asc,{CubejsConstants.TARGET}:asc,{CubejsConstants.METHOD}:asc";
        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_LIST_CHART_VIEW, where, orderBy, fields: [CubejsConstants.SERVICENAME, CubejsConstants.TARGET, CubejsConstants.METHOD, CubejsConstants.FAILED, CubejsConstants.LATENCY, CubejsConstants.THROUGHPUT, $"{CubejsConstants.TIMESTAMP_AGG}{{{CubejsConstants.TIMESTAMP_AGG_VALUE}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointListChartResponse>>(request);
        return response.Data.Data.Select(item => item.Data).ToList();
    }

    private ValueTuple<bool, DateTime, DateTime> SetAndCheckPreviousTime()
    {
        int day = 0;
        switch (Search.ComparisonType)
        {
            case ApmComparisonTypes.Day:
                day = -1;
                break;
            case ApmComparisonTypes.Week:
                day = -7;
                break;
        }
        if (day == 0)
            return (false, default, default);

        return (true, Search.Start.AddDays(day), Search.End.AddDays(day));
    }


    //private async Task GetPreviousChartData(BaseApmRequestDto query, bool isService, List<ChartLineDto> result, bool isPrevious)
    //{
    //    if (isService)
    //    {
    //        var request = GraphQLRequestUtils.GetServiceChartRequest(query);
    //        var list = await _client.SendQueryAsync<CubeListData<ServiceChartItemListResponse>>(request);
    //        SetChartData(result, list.Data.Items.Select(item => item.Data).ToList(), isService, isPrevious);
    //    }
    //    else
    //    {
    //        var request = GraphQLRequestUtils.GetEndpointChartRequest((ApmEndpointRequestDto)query);
    //        var list = await _client.SendQueryAsync<CubeListData<EndpointChartItemListResponse>>(request);
    //        SetChartData(result, list.Data.Items.Select(item => item.Data).ToList(), isService, isPrevious);
    //    }
    //}


    private void SetChartData(List<ChartLineDto> result, List<EndpointListChartItemResponse> data, bool isService, bool isPrevious = false)
    {
        ChartLineDto? current = null;
        var start = DateTime.Now;
        int index = 0;
        if (data == null || data.Count == 0) return;
        do
        {
            var item = data[index];
            string name;
            if (isService)//service
            {
                name = item.ServiceName;
            }
            else
            {
                name = $"{item.Method} {item.Target}";
            }
            var time = new DateTimeOffset(item.DateKey.Value).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineDto
                    {
                        Name = name,
                        Previous = new List<ChartLineItemDto>(),
                        Currents = new List<ChartLineItemDto>()
                    };
                    result.Add(current);
                }
            }

        ((List<ChartLineItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
            new()
            {
                Latency = (long)Math.Floor(item.Latency),
                Throughput = Math.Round((double)item.Throughput, 2, MidpointRounding.ToZero),
                Failed = Math.Round(item.Failed, 2, MidpointRounding.ToZero),
                //P99 = Math.Round(item.P99, 2, MidpointRounding.ToZero),
                //P95 = Math.Round(item.P95, 2, MidpointRounding.ToZero),
                Time = time
            });
            index++;
        } while (data.Count - index > 0);
    }









    private static EChartType ConvertLatencyChartData(ChartLineDto data, Func<ChartLineItemDto, object> fnProperty, string? lineColor = null, string? areaLineColor = null)
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
