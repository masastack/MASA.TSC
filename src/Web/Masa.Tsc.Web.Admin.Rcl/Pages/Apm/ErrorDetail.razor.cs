// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class ErrorDetail
{
    protected override bool IsPage => true;

    ChartData errorChart = new();

    LogResponseDto currentLog = null;

    TraceResponseDto currentTrace = null;

    int currentPage = 1;
    int total = 1;

    StringNumber index = 1;

    string search = string.Empty;
    IDictionary<string, object> _dic = null;
    bool loading = true;

    string exceptionType;
    string exceptionMessage;

    private async Task OnLoadAsync(SearchData data)
    {
        loading = true;
        Search = data;
        await LoadChartDataAsync();
        await ChangeRecordAsync();
        loading = false;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        exceptionType = query.Get("ex_type")!;
        exceptionMessage = query.Get("ex_msg")!;

        var append = new StringBuilder(Search.Text);

        if (!string.IsNullOrEmpty(exceptionType))
        {
            //append.AppendFormat(" and exception.type='{0}'", exceptionType);
            append.AppendFormat(" and LogAttributesValues[indexOf(LogAttributesKeys,'exception.type')]='{0}'", exceptionType);
        }
        //if (!string.IsNullOrEmpty(exceptionMessage))
        //{
        //    append.AppendFormat(" and LogAttributesValues[indexOf(LogAttributesKeys,'exception.message')]='{0}'", exceptionMessage);
        //}
        if (string.IsNullOrEmpty(Search.Text))
            append.Remove(0, 5);
        Search.Text = append.ToString();
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
            Query = Search.Text,
            Start = Search.Start,
            End = Search.End,
            IsLimitEnv = false
        });
        total = (int)result.Total;
        currentLog = result.Result[0];
        _dic = currentLog.ToDictionary();
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
        });
        if (result == null || result.Total == 0)
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
            //Queries = Search.Text,
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
}
