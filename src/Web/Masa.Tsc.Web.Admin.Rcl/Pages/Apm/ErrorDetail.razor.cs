// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class ErrorDetail
{
    [Parameter]
    public string Type { get; set; }

    [Parameter]
    public string Message { get; set; }

    [Parameter]
    public bool Show { get; set; }

    ChartData errorChart = new();

    LogResponseDto currentLog = null;

    TraceResponseDto currentTrace = null;

    [Inject]
    IJSRuntime JSRuntime { get; set; }

    IJSObjectReference module = null;

    int currentPage = 1;
    int total = 1;

    StringNumber index = 1;

    [Parameter]
    public SearchData SearchData { get => base.Search; set => base.Search = value; }

    string search = string.Empty;
    IDictionary<string, object> _dic = null;
    bool loading = true;
    string? lastKey = default, lastType = default, lastMessage = default;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/Masa.Tsc.Web.Admin.Rcl/Pages/Apm/ErrorDetail.razor.js");
        }
        else
        {
            await module.InvokeVoidAsync("autoHeight");
        }
    }

    private async Task OnLoadAsync(SearchData data = null)
    {
        loading = true;
        if (data != null)
        {
            currentPage = 1;
            total = 0;
            Search = data;
        }
        await ChangeRecordAsync();
        loading = false;
        await LoadChartDataAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!Show)
            return;
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(Search));
        if (lastKey != key || lastType != Type || lastMessage != Message)
        {
            lastKey = key;
            lastType = Type;
            lastMessage = Message;
            currentPage = 1;
            currentLog = default;
            _dic?.Clear();
            total = 1;
            await OnLoadAsync();
        }
    }

    private string GetText
    {
        get
        {
            var append = new StringBuilder();
            if (!string.IsNullOrEmpty(SearchData.TraceId))
                append.AppendFormat(" and `TraceId`='{0}'", SearchData.TraceId);
            if (!string.IsNullOrEmpty(Type))
                append.AppendFormat(" and `Attributes.exception.type`='{0}'", Type);
            if (!string.IsNullOrEmpty(Message))
                append.AppendFormat(" and `Body` like '{0}%'", Message.Replace("x2E", ".").Replace("'", "''").Split(':')[0]);
            append.Remove(0, 5);
            return append.ToString();
        }
    }

    private async Task LoadLogAysnc()
    {
        currentLog = default!;
        var result = await ApiCaller.LogService.GetPageAsync(new LogPageQueryDto
        {
            Service = Search.Service!,
            Env = Search.Environment!,
            PageSize = 1,
            Page = currentPage,
            Query = GetText,
            Start = Search.Start,
            End = Search.End,
            IsLimitEnv = false
        });
        if (currentPage == 1)
        {
            total = (int)result.Total;
        }

        if (result.Result == null || !result.Result.Any())
        {
            currentLog = null;
            _dic = new Dictionary<string, object>();
        }
        else
        {
            currentLog = result.Result[0];
            _dic = currentLog.ToDictionary();
        }
    }

    private async Task LoadTraceAsync()
    {
        currentTrace = default!;
        if (currentLog == null || string.IsNullOrEmpty(currentLog.SpanId) || !currentLog.Attributes.ContainsKey("RequestPath"))
            return;
        var result = await ApiCaller.TraceService.GetListAsync(new RequestTraceListDto
        {
            SpanId = currentLog.SpanId,
            Page = 1,
            PageSize = 1,
            Start = Search.Start,
            End = Search.End,
            Service = Search.Service!,
            HasPage = false
        });
        if (result == null || result.Result == null || !result.Result.Any())
            return;
        currentTrace = result.Result[0];
    }

    private async Task ChangePageAsync(int page)
    {
        loading = true;
        currentPage = page;
        await ChangeRecordAsync();
        loading = false;
    }

    private async Task ChangeRecordAsync()
    {
        await LoadLogAysnc();
        await LoadTraceAsync();
    }

    private async Task LoadChartDataAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = Search.Start,
            End = Search.End,
            Queries = GetText,
            Service = Search.Service,
            Endpoint = Search.Endpoint!,
            Env = Search.Environment,
        };
        var result = await ApiCaller.ApmService.GetErrorChartAsync(query);
        errorChart.Data = ConvertLatencyChartData(result, lineName: "error count").Json;
        errorChart.ChartLoading = false;
    }

    private EChartType ConvertLatencyChartData(List<ChartLineCountDto> data, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
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
                new { type="category",boundaryGap=false,data=data?.Select(item=>item.Currents.First().Time.ToDateTime(CurrentTimeZone).Format()) }
            });
            chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Select(item => item.Currents.First().Value) });
        }

        return chart;
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore();
        if (module != null)
            await module.DisposeAsync();
    }
}
