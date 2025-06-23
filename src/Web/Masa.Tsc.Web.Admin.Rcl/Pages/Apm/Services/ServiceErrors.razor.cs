// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Services;

public partial class ServiceErrors
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    [Parameter]
    public string SpanId { get; set; }

    [Parameter]
    public bool ShowChart { get; set; } = true;

    [Parameter]
    public string[] TraceIds { get; set; }

    [Parameter]
    public MetricTypes MetricType { get; set; }

    private List<DataTableHeader<ErrorMessageDto>> headers => new()
    {
        new() { Text = I18n.Apm("Error.List.Type"), Value = nameof(ErrorMessageDto.Type)},
        new() { Text = I18n.Apm("Error.List.Message"), Value = nameof(ErrorMessageDto.Message) },
        new() { Text = I18n.Apm("Error.List.LastTime"), Value = nameof(ErrorMessageDto.LastTime) },
        new() { Text = I18n.Apm("Error.List.Total"), Value = nameof(ErrorMessageDto.Total)}
    };

    private int defaultSize = 20;
    private int total = 0;
    private int page = 1;
    private List<ErrorMessageDto> data = new();
    private bool isTableLoading = false;
    private string sortFiled = nameof(ErrorMessageDto.Total);
    private bool sortBy = true;
    private string lastKey = string.Empty, lastSpanId = string.Empty;
    private ChartData chart = new();

    private async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy[0];
        else
            sortFiled = default!;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc[0];
        else
            sortBy = default;
        await LoadASync();
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        var key = Encrypt(JsonSerializer.Serialize(SearchData) + JsonSerializer.Serialize(TraceIds));
        if (lastKey != key || SpanId != null && SpanId != lastSpanId || SpanId == null && lastSpanId.Length > 0)
        {
            lastKey = key;
            lastSpanId = SpanId ?? string.Empty;
            await LoadASync();
            StateHasChanged();
            //await LoadChartDataAsync();
            await LoadCubeChartDataAsync();
        }
        await base.OnParametersSetAsync();
    }

    private StringNumber GetHeight()
    {
        return ShowChart ? "calc(100vh - 620px)" : "calc(100vh - 420px)";
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadPageDataAsync();
    }

    private async Task LoadASync()
    {
        await LoadCubePageDataAsync();
    }
    
    private async Task LoadCubePageDataAsync()
    {
        if (string.IsNullOrEmpty(SearchData.Service) && string.IsNullOrEmpty(SearchData.TraceId))
        {
            total = 0;
            data = [];
            return;
        }
        if (isTableLoading) return;
        isTableLoading = true;
        var traceId = !string.IsNullOrEmpty(SearchData.TextValue) && SearchData.TextField == StorageConst.Current.TraceId ? SearchData.TextValue : null;
        var where = CubeJsRequestUtils.GetErrorChartWhere(SearchData.Start, SearchData.End, default, SearchData.Service!, default!, default!, default, traceId!, SpanId, TraceIds);
        var orderBy = $"{CubejsConstants.TIMESTAMP_AGG}:asc";

        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_ERROR_LIST_VIEW, where, orderBy, fields: [CubejsConstants.ERROR_PAGE_COUNT]));
        var responseTotal = await CubejsClient.SendQueryAsync<CubejsBaseResponse<ServiceErrorResponse<ServiceErrorListItemTotalResponse>>>(request);
        if (responseTotal.Data != null && responseTotal.Data.Data != null && responseTotal.Data.Data.Count > 0)
        {
            total = responseTotal.Data.Data[0].Data.GrCnt;
        }
        else
        {
            total = 0;
            data = [];
            isTableLoading = false;
            return;
        }

        request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_ERROR_LIST_VIEW, where, orderBy, page, defaultSize, fields: [CubejsConstants.ERROR_TYPE, CubejsConstants.ERROR_MESSAGE, CubejsConstants.COUNT, CubejsConstants.ERROR_PAGE_MAX_TIME]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<ServiceErrorResponse<ServiceErrorListItemResponse>>>(request);
        if (response.Data != null && response.Data.Data != null && response.Data.Data.Count > 0)
        {
            data = response.Data.Data.Select(item => new ErrorMessageDto
            {
                Type = item.Data.ExceptionType,
                Message = item.Data.MsgGroupKey,
                Total = item.Data.Cnt,
                LastTime = item.Data.MaxT
            }).ToList();
        }
        else
        {
            data = [];
        }
        isTableLoading = false;
    }

    private async Task LoadCubeChartDataAsync()
    {
        if (!ShowChart)
            return;
        List<ChartLineCountDto> result = [new() { Previous = new List<ChartLineCountItemDto>(), Currents = new List<ChartLineCountItemDto>() }];

        var traceId = !string.IsNullOrEmpty(SearchData.TextValue) && SearchData.TextField == StorageConst.Current.TraceId ? SearchData.TextValue : null;
        var list = await GetChartDataAsync(Search.Start, Search.End, traceId);

        SetChartData(result, list, false);
        //(bool hasPrious, DateTime start, DateTime end) = SetAndCheckPreviousTime();
        //if (hasPrious && string.IsNullOrEmpty(traceId))
        //{
        //    var previousList = await GetChartDataAsync(start, end);
        //    SetChartData(result, previousList, true);
        //}
        chart.Data = ConvertLatencyChartData(result.Count > 0 ? result[0] : default, lineName: I18n.Apm("Chart.ErrorCount")).Json;
        chart.ChartLoading = false;
        StateHasChanged();
    }

    private async Task<List<ServiceErrorChartResponse>> GetChartDataAsync(DateTime start, DateTime end, string? traceId = default)
    {
        if (!string.IsNullOrEmpty(Search.Endpoint) && !string.IsNullOrEmpty(Search.Method))
            return await GetEndpointChartDataAsync(start, end, traceId);
        return await GetServiceChartDataAsync(start, end, traceId);
    }

    private async Task<List<ServiceErrorChartResponse>> GetServiceChartDataAsync(DateTime start, DateTime end, string? traceId = default)
    {
        var where = CubeJsRequestUtils.GetErrorChartWhere(start, end, SearchData.Environment, SearchData.Service!, default!, default!, default, traceId, SpanId, TraceIds);
        var orderBy = $"{CubejsConstants.TIMESTAMP_AGG}:asc";
        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_ERROR_LIST_VIEW, where, orderBy, fields: [CubejsConstants.COUNT, $"{CubejsConstants.TIMESTAMP_AGG}{{{CubeJsRequestUtils.GetCubeTimePeriod(SearchData.Start, SearchData.End)}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<ServiceErrorResponse<ServiceErrorChartResponse>>>(request);
        if (response.Data != null && response.Data.Data != null && response.Data.Data.Count > 0)
        {
            return response.Data.Data.Select(item => item.Data).ToList();
        }
        else
        {
            return [];
        }
    }

    private async Task<List<ServiceErrorChartResponse>> GetEndpointChartDataAsync(DateTime start, DateTime end, string? traceId = default)
    {
        var where = CubeJsRequestUtils.GetErrorChartWhere(start, end, SearchData.Environment, SearchData.Service!, SearchData.Endpoint!, SearchData.Method!, SearchData.Status, traceId, SpanId);
        var orderBy = $"{CubejsConstants.TIMESTAMP_AGG}:asc";
        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_ERROR_CHART_VIEW, where, orderBy, fields: [CubejsConstants.COUNT, $"{CubejsConstants.TIMESTAMP_AGG}{{{CubeJsRequestUtils.GetCubeTimePeriod(SearchData.Start, SearchData.End)}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointDetailErrorResponse<ServiceErrorChartResponse>>>(request);
        if (response.Data != null && response.Data.Data != null && response.Data.Data.Count > 0)
        {
            return response.Data.Data.Select(item => item.Data).ToList();
        }
        else
        {
            return [];
        }
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

    private void SetChartData(List<ChartLineCountDto> result, List<ServiceErrorChartResponse> data, bool isPrevious = false)
    {
        ChartLineCountDto current = result[0];
        int index = 0;
        if (data == null || data.Count == 0) return;
        do
        {
            var item = data[index];
            var time = item.DateKey.DateTime!.Value.ToUnixTimestamp();
            ((List<ChartLineCountItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Value = item.Cnt.ToString(),
                    Time = time
                });
            index++;
        } while (data.Count - index > 0);
    }

    private async Task LoadChartDataAsync()
    {
        if (!ShowChart)
            return;
        List<ChartLineCountDto> result = null;
        if (!string.IsNullOrEmpty(SearchData.Service))
        {
            var query = new ApmEndpointRequestDto
            {
                Page = page,
                PageSize = defaultSize,
                Start = SearchData.Start,
                End = SearchData.End,
                OrderField = sortFiled,
                Service = SearchData.Service,
                Endpoint = SearchData.Endpoint!,
                Env = SearchData.Environment,
                IsDesc = sortBy
            };
            result = await ApiCaller.ApmService.GetErrorChartAsync(query);
        }
        chart.Data = ConvertLatencyChartData(result.Count > 0 ? result[0] : default, lineName: I18n.Apm("Chart.ErrorCount")).Json;
        chart.ChartLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        if (isTableLoading) return;
        isTableLoading = true;
        if (string.IsNullOrEmpty(SearchData.Service) && string.IsNullOrEmpty(SearchData.TraceId))
        {
            total = 0;
            data = new();
        }
        else
        {
            var query = new ApmErrorRequestDto
            {
                Page = page,
                PageSize = defaultSize,
                Start = SearchData.Start,
                End = SearchData.End,
                OrderField = sortFiled,
                TraceId = SearchData.TraceId,
                Service = SearchData.Service,
                Env = SearchData.Environment,
                IsDesc = sortBy,
                TextField = StorageConst.Current.SpanId,
                TextValue = lastSpanId,
                Filter = SearchData.EnableExceptError
            };
            var result = await ApiCaller.ApmService.GetErrorsPageAsync(CurrentTeamId, query, SearchData.Project, SearchData.ServiceType, string.IsNullOrEmpty(SearchData.Service));
            data = result?.Result?.ToList() ?? new();
            total = (int)(result?.Total ?? 0);
        }
        isTableLoading = false;
    }

    //private EChartType ConvertLatencyChartData(List<ChartLineCountDto> data, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
    //{
    //    var chart = EChartConst.Line;
    //    chart.SetValue("tooltip", new { trigger = "axis" });
    //    if (!string.IsNullOrEmpty(lineName))
    //    {
    //        chart.SetValue("legend", new { data = new string[] { $"{lineName}" }, bottom = "2%" });
    //    }

    //    chart.SetValue("yAxis", new object[] {
    //        new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
    //    });
    //    chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
    //    //if (data != null && data.Any())
    //    {
    //        chart.SetValue("xAxis", new object[] {
    //            new { type="category",boundaryGap=false,data=data?.Select(item=>item.Currents.First().Time.ToDateTime(CurrentTimeZone).Format()) }
    //        });
    //        chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Select(item => item.Currents.First().Value) });
    //    }
    //}


    private EChartType ConvertLatencyChartData(ChartLineCountDto? data, string? lineName = null, string? unit = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { trigger = "axis" });
        if (!string.IsNullOrEmpty(lineName))
        {
            chart.SetValue("legend", new { data = new string[] { $"{lineName}" }, bottom = "2%" });
        }

        chart.SetValue("yAxis", new object[] {
            new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        });
        chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        //if (data != null && data.Any())
        {
            chart.SetValue("xAxis", new object[] {
                new { type="category",boundaryGap=false,data=data?.Currents?.Select(item=>item.Time.ToDateTime(CurrentTimeZone).Format())??[] }
            });
            chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Currents.Select(item => item.Value) ?? [] });
        }


        //var chart = EChartConst.Line;
        //chart.SetValue("tooltip", new { trigger = "axis" });
        //if (!string.IsNullOrEmpty(lineName))
        //{
        //    chart.SetValue("legend", new { data = new string[] { $"current {lineName}", $"previous {lineName}" }, bottom = "2%" });
        //}
        //chart.SetValue("xAxis", new object[] {
        //    new { type="category",boundaryGap=false,data=data?.Currents?.Select(item=>item.Time)??[]}
        //});
        //chart.SetValue("yAxis", new object[] {
        //    new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        //});
        //chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        //var index = 0;
        //chart.SetValue($"series[{index++}]", new { name = $"current {lineName}", type = "line", smooth = true, symbol = "none", data = data?.Currents?.Select(item => item.Value) ?? [] });
        //chart.SetValue($"series[{index}]", new { name = $"previous {lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Previous?.Select(item => item.Value) ?? [] });
        //return chart;



        ////return chart;        
        //var chart = EChartConst.Line;
        //chart.SetValue("tooltip", new { });
        //chart.SetValue("legend", new { });
        //chart.SetValue("xAxis.show", false);
        //chart.SetValue("yAxis.show", false);
        //chart.SetValue("grid", new { top = "0%", left = "0%", right = "0%", bottom = "0%" });
        //var index = 0;

        //chart.SetValue($"series[{index++}]", new { type = "line", smooth = true, symbol = "none", data = data?.Currents?.Select(x => x.Time) ?? [] });
        //chart.SetValue($"series[{index}]", new { type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Previous?.Select(x => x.Value) ?? [] });

        return chart;

    }

    bool showDetail = false;

    private void Show(string? type = default, string? message = default)
    {
        Search.ExceptionType = type!;
        Search.ExceptionMsg = message!;
        if (!string.IsNullOrEmpty(SearchData.TraceId))
            Search.TraceId = SearchData.TraceId;
        else
            Search.TraceId = string.Empty;
        showDetail = true;
        StateHasChanged();
    }
}
