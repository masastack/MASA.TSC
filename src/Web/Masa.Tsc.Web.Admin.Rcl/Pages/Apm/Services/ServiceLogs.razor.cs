// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Services;

public partial class ServiceLogs
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    [Parameter]
    public bool ShowChart { get; set; } = true;

    [Parameter]
    public MetricTypes MetricType { get; set; }

    [Parameter]
    public string SpanId { get; set; }

    [Parameter]
    public string[] TraceIds { get; set; }

    [Parameter]
    public bool ShowAppEvent { get; set; } = true;

    private List<DataTableHeader<LogResponseDto>> headers => new()
    {
        new() { Text = I18n.Apm("Log.List.Timestamp"), Value = nameof(LogResponseDto.Timestamp)},
        new() { Text = I18n.Apm("Log.List.TraceId"), Value = nameof(LogResponseDto.TraceId)},
        new() { Text = I18n.Apm("Log.List.SeverityText"), Value = nameof(LogResponseDto.SeverityText)},
        new() { Text = I18n.Apm("Log.List.Body"), Value = nameof(LogResponseDto.Body) }
    };

    private int defaultSize = 20;
    private int total = 0;
    private int page = 1;
    private List<LogResponseDto> data = new();
    private bool isTableLoading = false;
    private string? sortFiled = nameof(LogResponseDto.Timestamp);
    private bool sortBy = true;
    private string lastKey = string.Empty, lastSpanId = string.Empty;
    private readonly ChartData chart = new();
    private bool dialogShow = false;
    private LogResponseDto current = null;

    private async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy[0];
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc[0];
        else
            sortBy = default;
        await LoadAsync();
        StateHasChanged();
    }

    private void OpenAsync(LogResponseDto item)
    {
        current = item;
        dialogShow = true;
    }

    private StringNumber GetHeight()
    {
        return ShowChart ? "calc(100vh - 620px)" : "calc(100vh - 420px)";
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        var key = Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key || SpanId != null && SpanId != lastSpanId || SpanId == null && lastSpanId.Length > 0)
        {
            lastKey = key;
            lastSpanId = SpanId ?? string.Empty;
            await LoadAsync();
            await LoadCubeChartDataAsync();
            //await LoadChartDataAsync();
        }
    }

    private async Task LoadAsync(SearchData data = null)
    {
        if (data != null)
            SearchData = data;
        if (SearchData.Start == DateTime.MinValue || SearchData.End == DateTime.MinValue)
            return;
        await LoadCubePageDataAsync();
        //await LoadPageDataAsync();
    }

    private async Task LoadChartDataAsync()
    {
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
            result = await ApiCaller.ApmService.GetLogChartAsync(query);
        }
        //chart.Data = ConvertLatencyChartData(result, lineName: "log count").Json;
        chart.ChartLoading = false;
    }

    private async Task LoadCubeChartDataAsync()
    {
        if (!ShowChart) return;
        ChartLineCountDto result = new ChartLineCountDto { Currents = new List<ChartLineCountItemDto>() };
        var traceId = !string.IsNullOrEmpty(SearchData.TextValue) && SearchData.TextField == StorageConst.Current.TraceId ? SearchData.TextValue : null;

        var where = CubeJsRequestUtils.GetErrorChartWhere(SearchData.Start, SearchData.End, SearchData.Environment, SearchData.Service!, default!, default!, default, traceId, SpanId, TraceIds);
        var orderBy = $"{CubejsConstants.TIMESTAMP_AGG}:asc";
        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_LOG_DETAIL_VIEW, where, orderBy, fields: [CubejsConstants.COUNT, $"{CubejsConstants.TIMESTAMP_AGG}{{{CubeJsRequestUtils.GetCubeTimePeriod(SearchData.Start, SearchData.End)}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointDetailLogResponse<EndpointDetailLogChartItemResponse>>>(request);
        if (response.Data != null && response.Data.Data != null && response.Data.Data.Count > 0)
        {
            ((List<ChartLineCountItemDto>)result.Currents).AddRange(response.Data.Data.Select(item => new ChartLineCountItemDto
            {
                Time = item.Data.DateKey.DateTime!.Value.ToUnixTimestamp(),
                Value = item.Data.Cnt
            }));
        }

        chart.Data = ConvertLatencyChartData(result, lineName: "log count").Json;
        chart.ChartLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        if (isTableLoading) return;
        isTableLoading = true;
        if (string.IsNullOrEmpty(SearchData.Service) && string.IsNullOrEmpty(SearchData.TraceId))
        {
            data.Clear();
            total = 0;
        }
        else
        {
            var query = new BaseRequestDto
            {
                Page = page,
                PageSize = defaultSize,
                Start = SearchData.Start,
                End = SearchData.End,
                TraceId = SearchData.TraceId,
                Service = SearchData.Service,
                Sort = new FieldOrderDto
                {
                    IsDesc = sortBy,
                    Name = sortFiled!
                },

                Endpoint = SearchData.Endpoint!
            };
            var list = new List<FieldConditionDto>();
            if (!string.IsNullOrEmpty(SearchData.Environment))
                list.Add(new FieldConditionDto { Value = SearchData.Environment!, Name = StorageConst.Current.Environment });
            if (!ShowAppEvent)
            {
                list.Add(new FieldConditionDto
                {
                    Name = StorageConst.Current.Log.Body,
                    Value = "Event",
                    Type = ConditionTypes.NotRegex
                });
            }
            if (!string.IsNullOrEmpty(lastSpanId))
            {
                list.Add(new FieldConditionDto
                {
                    Name = StorageConst.Current.SpanId,
                    Value = lastSpanId,
                    Type = ConditionTypes.Equal
                });
            }
            if (Search.EnableExceptError)
                list.Add(new FieldConditionDto { Name = nameof(ApmErrorRequestDto.Filter), Value = true, Type = ConditionTypes.Equal });
            query.Conditions = list;
            var result = await ApiCaller.ApmService.GetLogListAsync(CurrentTeamId, query, SearchData.Project, SearchData.ServiceType, string.IsNullOrEmpty(SearchData.Service));
            data.Clear();
            total = 0;
            if (result != null)
            {
                if (result.Result != null && result.Result.Any())
                {
                    data.AddRange(result.Result);
                }
                total = (int)result.Total;
            }
        }
        isTableLoading = false;
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
        var where = CubeJsRequestUtils.GetErrorChartWhere(SearchData.Start, SearchData.End, default, SearchData.Service!, default!, default!, default, traceId, SpanId, TraceIds);
        var orderBy = CubeJsRequestUtils.GetEndpintLogsOrderBy(sortFiled!, sortBy);

        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_LOG_DETAIL_VIEW, where, orderBy, 1, 1, fields: [CubejsConstants.COUNT]));
        var responseTotal = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointDetailLogResponse<EndpointDetailLogListTotalResponse>>>(request);
        if (responseTotal.Data != null && responseTotal.Data.Data != null && responseTotal.Data.Data.Count > 0)
        {
            total = responseTotal.Data.Data[0].Data.Cnt;
        }
        else
        {
            total = 0;
            data = [];
            isTableLoading = false;
            return;
        }

        request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_LOG_DETAIL_VIEW, where, orderBy, page, defaultSize, fields: [CubejsConstants.TRACEID, CubejsConstants.SPANID, "severitytext", "severitynumber", "body", "resources", "logs", $"{CubejsConstants.TIMESTAMP_AGG}{{{CubejsConstants.TIMESTAMP_AGG_VALUE}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointDetailLogResponse<EndpointDetailLogListItemResponse>>>(request);
        if (response.Data != null && response.Data.Data != null && response.Data.Data.Count > 0)
        {
            data = response.Data.Data.Select(item => new LogResponseDto
            {
                SpanId = item.Data.SpanId,
                TraceId = item.Data.TraceId,
                Body = item.Data.Body,
                SeverityText = item.Data.SeverityText,
                SeverityNumber = int.Parse(item.Data.SeverityNumber),
                Timestamp = item.Data.DateKey.DateTime!.Value,
                Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data.Logs)!,
                Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data.Resources)!
            }).ToList();
        }
        else
        {
            data = [];
        }
        isTableLoading = false;
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadAsync();
    }

    private EChartType ConvertLatencyChartData(ChartLineCountDto data, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { trigger = "axis" });
        if (!string.IsNullOrEmpty(lineName))
        {
            chart.SetValue("legend", new { data = new string[] { $"{lineName}" }, bottom = "2%" });
        }
        chart.SetValue("xAxis", new object[] {
            new { type="category",boundaryGap=false,data=data?.Currents?.Select(item=>item.Time.ToDateTime(CurrentTimeZone).Format())??[] }
        });
        chart.SetValue("yAxis", new object[] {
            new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        });
        chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        //if (data.Previous != null && data.Previous.Any())
        {
            chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Currents?.Select(item => item.Value) ?? [] });
        }

        return chart;
    }
}