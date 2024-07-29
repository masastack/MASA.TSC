// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
    private readonly List<LogResponseDto> data = new();
    private bool isTableLoading = false;
    private string? sortFiled = nameof(LogResponseDto.Timestamp);
    private bool sortBy = true;
    private string lastKey = string.Empty;
    private readonly ChartData chart = new();
    private bool dialogShow = false;
    private LogResponseDto current = null;

    private async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy.First();
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc.First();
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
        return ShowChart ? 300 : 500;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key)
        {
            lastKey = key;
            await LoadAsync();
            await LoadChartDataAsync();
        }
    }

    private async Task LoadAsync(SearchData data = null)
    {
        if (data != null)
            SearchData = data;
        if (isTableLoading)
        {
            return;
        }
        await LoadPageDataAsync();
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
            query.Conditions = list;
            var result = await ApiCaller.ApmService.GetLogListAsync(query);
            data.Clear();
            if (result.Result != null && result.Result.Any())
            {
                data.AddRange(result.Result);
            }
            total = (int)result.Total;
        }
        isTableLoading = false;
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadPageDataAsync();
    }

    private EChartType ConvertLatencyChartData(List<ChartLineCountDto> data, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { trigger = "axis" });
        if (!string.IsNullOrEmpty(lineName))
        {
            chart.SetValue("legend", new { data = new string[] { $"{lineName}" }, bottom = "2%" });
        }
        chart.SetValue("xAxis", new object[] {
            new { type="category",boundaryGap=false,data=data?.Select(item=>item.Currents.First().Time.ToDateTime(CurrentTimeZone).Format()) }
        });
        chart.SetValue("yAxis", new object[] {
            new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        });
        chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        //if (data.Previous != null && data.Previous.Any())
        {
            chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Select(item => item.Currents.First().Value) });
        }

        return chart;
    }
}