// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Endpoints;

public partial class OverView
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }
    IJSObjectReference? echartEventModule = null;
    DotNetObjectReference<OverView> objRef;

    [CascadingParameter]
    public SearchData SearchData { get; set; }

    [Parameter]
    public EventCallback<string> OnSpanIdChanged { get; set; }

    private async Task SpanIdChange(string spanId)
    {
        if (OnSpanIdChanged.HasDelegate)
            await OnSpanIdChanged.InvokeAsync(spanId);
    }

    private static readonly List<(MetricTypes, string)> metricTypes = new() { (MetricTypes.Avg, "avg"), (MetricTypes.P95, "p95"), (MetricTypes.P99, "p99") };
    private readonly LatencyTypeChartData metricTypeChartData = new();
    private ChartData throughput = new();
    private ChartData failed = new();
    private readonly ChartData timeTypeCount = new();
    private string? lastKey = null;
    private List<TraceResponseDto>? traceDetails = null;
    private List<ChartPointDto>? errors = null;
    int page = 1, total = 1;
    readonly Dictionary<double, int> latencies = new();
    double percentile = 0;
    private List<SimpleTraceListDto> traceIds = new();
    private List<SimpleTraceListDto> traceTails = new();
    MECharts? mechart;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        objRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (mechart != null)
        {
            await Task.Delay(1200);
            var echart = typeof(MECharts).GetField("_echarts", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)!.GetValue(mechart)!;
            if (echart != null)
            {
                var reference = echart.GetType().GetField("_selfReference", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)!.GetValue(echart);
                var echartJs = (IJSObjectReference)typeof(JSObjectReferenceProxy).GetField("_jsObjectReference", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)!.GetValue(echart)!;

                if (reference != null && echartJs != null && echartEventModule == null && echartEventModule == null)
                {
                    //重新注册
                    echartEventModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/Masa.Tsc.Web.Admin.Rcl/Pages/Apm/Endpoints/OverView.razor.js");
                    await echartEventModule.InvokeVoidAsync("setChartEvent", echartJs, objRef);
                }
            }
            else
            {
                if (echartEventModule != null)
                {
                    await echartEventModule.DisposeAsync();
                    echartEventModule = null;
                }
            }
        }
    }

    CancellationTokenSource source = null;
    private EChartBrushEventArg lastSelect = new(true);
    [JSInvokable("OnBrushEnd")]
    public async Task BrushEnd(EChartBrushEventArg args)
    {
        bool isRefresh = false;
        if (args.IsClear && !lastSelect.IsClear)
        {
            lastSelect.IsClear = false;
            isRefresh = true;
        }
        else if (!args.IsClear && (lastSelect.IsClear || !lastSelect.IsClear && (lastSelect.Start - args.Start != 0 || lastSelect.End - args.Start != 0)))
        {
            lastSelect.IsClear = false;
            lastSelect.Start = args.Start;
            lastSelect.End = args.End;
            isRefresh = true;
        }

        if (isRefresh)
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            await RefreshDurationListAsync(source.Token);
            await InvokeAsync(StateHasChanged);
            source?.Dispose();
            source = null;
        }
    }

    private async Task RefreshDurationListAsync(CancellationToken token)
    {
        await Task.Delay(400, token);
        total = 0;
        if (token.IsCancellationRequested)
            return;
        await LoadTraceDetailAsync(1);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!SearchData.Loaded)
            return;
        var text = JsonSerializer.Serialize(SearchData);
        var key = Encrypt(text);
        if (lastKey != key)
        {
            lastKey = key;
            lastSelect.IsClear = true;
            //await LoadChartDataAsync();
            await LoadCubeChartDataAsync();
            await LoadDistributionDataAsync();
        }
    }

    private async Task LoadTraceDetailAsync(int page = 1)
    {
        this.page = page;
        SimpleTraceListDto trace = null;
        traceDetails = null;
        errors = null;
        percentile = 0;
        //首次
        if (total == 0 && page == 1)
        {
            traceIds.Clear();
            traceTails.Clear();
            var query = new ApmTraceLatencyRequestDto
            {
                Start = SearchData.Start,
                End = SearchData.End,
                Endpoint = SearchData.Endpoint!,
                Service = SearchData.Service!,
                Env = SearchData.Environment!,
                Page = 1,
                Method = SearchData.Method,
                TextField = SearchData.TextField,
                TextValue = SearchData.TextValue,
                StatusCode = SearchData.Status,
                //Queries = SearchData.Text,
                OrderField = "Timestamp",
                IsDesc = true
            };
            if (lastSelect != null && !lastSelect.IsClear)
            {
                var isEqal = lastSelect.Start == lastSelect.End;
                int start = chartData[lastSelect.Start][0].FormatTimeToNumber(), end = chartData[lastSelect.End][0].FormatTimeToNumber(isEqal);
                query.LatMin = start;
                query.LatMax = end;
                //Console.WriteLine($"start:{query.LatMin},end:{query.LatMax}");
            }

            var result1 = await ApiCaller.ApmService.GetSimpleTraceListAsync(query);
            if (result1 != null && result1.Result != null)
            {
                traceIds.AddRange(result1.Result);
                total = (int)result1.Total;
                if (total - 500 > 0) total = 500;
            }
        }

        if (traceIds.Count - page >= 0)
            trace = traceIds[page - 1];
        else if (traceTails.Count > 0)
            trace = traceTails[page - 1 - (total - traceTails.Count)];

        if (trace != null)
        {
            traceDetails = (await ApiCaller.TraceService.GetAsync(trace.TraceId, trace.Timestamp.AddHours(-6), trace.EndTimestamp.AddHours(6)))?.ToList()!;
            CaculatePercentil();
            StateHasChanged();
            await LoadTraceErrorsAsync(trace.TraceId);
            StateHasChanged();
        }
    }

    private async Task LoadTraceErrorsAsync(string traceId)
    {
        var queryError = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Env = SearchData.Environment,
            Queries = $"and TraceId='{traceId}'",
            Page = 1,
            PageSize = 100
        };
        errors = await ApiCaller.ApmService.GetSpanErrorsAsync(queryError);
    }

    private void CaculatePercentil()
    {
        if (traceDetails == null || !traceDetails.Any())
            return;
        var current = traceDetails.Find(item => (item.Kind == "SPAN_KIND_SERVER" || item.Kind == "Server")
                && item.Attributes.TryGetValue("http.target", out var url) && string.Equals(SearchData.Endpoint, ((JsonElement)url).GetString())
                && item.Resource.TryGetValue("service.name", out var service) && string.Equals(SearchData.Service, ((JsonElement)service).GetString())
                )?.Duration;
        if (!current.HasValue)
            return;
        int sum = latencies.Sum(item => item.Value);
        int lessTotal = 0;
        foreach (var key in latencies.Keys)
        {
            if (key - current > 0)
                break;
            lessTotal += latencies[key];
        }
        percentile = lessTotal * 1.0 / sum;
    }




    private async Task LoadCubeChartDataAsync()
    {
        //var teamId = CurrentTeamId;
        var teamId = Guid.Parse("77ad20db-729f-4120-bf9c-6978f2d0ec2c");

        var result = new List<ChartLineDto>() { new ChartLineDto() { Currents = new List<ChartLineItemDto>(), Previous = new List<ChartLineItemDto>() } };
        if (!string.IsNullOrEmpty(Search.Endpoint))
        {
            var list = await GetChartDataAsync(Search.Start, Search.End);
            SetChartData(result, list, false);
            (bool hasPrious, DateTime start, DateTime end) = SetAndCheckPreviousTime();
            if (hasPrious)
            {
                var previousList = await GetChartDataAsync(start, end);
                SetChartData(result, previousList, true);
            }
        }
        SetChartUiJson(result[0]);
    }

    private async Task<List<EndpointDetailChartItemResponse>> GetChartDataAsync(DateTime start, DateTime end)
    {
        var where = CubeJsRequestUtils.GetEndpintDetailChartWhere(start, end, Search.Environment, Search.Service!, Search.Endpoint!, Search.Method);
        var orderBy = $"{CubejsConstants.TIMESTAMP_AGG}:asc";
        var request = new GraphQLHttpRequest(CubeJsRequestUtils.GetCompleteCubejsQuery(CubejsConstants.ENDPOINT_DETAIL_CHART_VIEW, where, orderBy, fields: [CubejsConstants.FAILED, CubejsConstants.LATENCY, CubejsConstants.THROUGHPUT, CubejsConstants.P95, CubejsConstants.P99, $"{CubejsConstants.TIMESTAMP_AGG}{{{CubejsConstants.TIMESTAMP_AGG_VALUE}}}"]));
        var response = await CubejsClient.SendQueryAsync<CubejsBaseResponse<EndpointDetailChartResponse>>(request);
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

    private void SetChartData(List<ChartLineDto> result, List<EndpointDetailChartItemResponse> data, bool isPrevious = false)
    {
        ChartLineDto? current = result[0];
        var start = DateTime.Now;
        int index = 0;
        if (data == null || data.Count == 0) return;
        do
        {
            var item = data[index];
            var name = item.DateKey.Value;
            var time = new DateTimeOffset(name).ToUnixTimeSeconds();
            ((List<ChartLineItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Latency = item.Latency,
                    Throughput = Math.Round((double)item.Throughput, 2, MidpointRounding.ToZero),
                    Failed = Math.Round(item.Failed, 2, MidpointRounding.ToZero),
                    P99 = item.PNinetyNine,
                    P95 = item.PNinetyNine,
                    Time = time
                });
            index++;
        } while (data.Count - index > 0);
    }

    private async Task LoadChartDataAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Service = SearchData.Service,
            Env = SearchData.Environment,
            Endpoint = SearchData.Endpoint!,
            Method = SearchData.Method,
            //Queries = SearchData.Text,
            ComparisonType = SearchData.ComparisonType.ToComparisonType()
        };
        var data = await ApiCaller.ApmService.GetChartsAsync(query);
        SetChartUiJson(data?.FirstOrDefault());
    }

    private void SetChartUiJson(ChartLineDto data)
    {
        if (data != null && data.Currents != null && data.Currents.Any())
        {
            metricTypeChartData.Avg = new();
            metricTypeChartData.P95 = new();
            metricTypeChartData.P99 = new();
            throughput = new();
            failed = new();

            metricTypeChartData.Avg.Data = ConvertLatencyChartData(data, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Latency, unit: "ms", lineName: "Average").Json;
            metricTypeChartData.P95.Data = ConvertLatencyChartData(data, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.P95, unit: "ms", lineName: "95th percentile").Json;
            metricTypeChartData.P99.Data = ConvertLatencyChartData(data, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.P99, unit: "ms", lineName: "99th percentile").Json;
            throughput.Data = ConvertLatencyChartData(data, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Throughput, unit: "tpm").Json;
            failed.Data = ConvertLatencyChartData(data, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Failed, unit: "%").Json;

            metricTypeChartData.Avg.ChartLoading = false;
            metricTypeChartData.P95.ChartLoading = false;
            metricTypeChartData.P99.ChartLoading = false;
            throughput.ChartLoading = false;
            failed.ChartLoading = false;

        }
        else
        {
            metricTypeChartData.Avg.HasChart = false;
            metricTypeChartData.P95.HasChart = false;
            metricTypeChartData.P99.HasChart = false;
            throughput.HasChart = false;
            failed.HasChart = false;
        }
        StateHasChanged();
    }

    private static EChartType ConvertLatencyChartData(ChartLineDto data, Func<ChartLineItemDto, object> fnXProperty, Func<ChartLineItemDto, object> fnProperty, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { trigger = "axis" });
        if (!string.IsNullOrEmpty(lineName))
        {
            chart.SetValue("legend", new { data = new string[] { $"current {lineName}", $"previous {lineName}" }, bottom = "2%" });
        }
        chart.SetValue("xAxis", new object[] {
            new { type="category",boundaryGap=false,data=data.Currents.Select(item=>fnXProperty(item))}
        });
        chart.SetValue("yAxis", new object[] {
            new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        });
        chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        var index = 0;
        if (data.Currents != null && data.Currents.Any())
        {
            chart.SetValue($"series[{index++}]", new { name = $"current {lineName}", type = "line", smooth = true, symbol = "none", data = data.Currents.Select(fnProperty) });
        }
        if (data.Previous != null && data.Previous.Any())
        {
            chart.SetValue($"series[{index}]", new { name = $"previous {lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data.Previous.Select(fnProperty) });
        }

        return chart;
    }

    private async Task LoadDistributionDataAsync()
    {
        latencies.Clear();
        total = 0;
        timeTypeCount.ChartLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Service = SearchData.Service,
            Env = SearchData.Environment,
            Endpoint = SearchData.Endpoint!,
            Method = SearchData.Method,
            //Queries = SearchData.Text,
            ComparisonType = SearchData.ComparisonType.ToComparisonType()
        };
        await LoadTraceDetailAsync();
        StateHasChanged();
        var data = await ApiCaller.ApmService.GetLatencyDistributionAsync(query);
        var currentSpan = traceDetails?.Find(item => item.Attributes.TryGetValue("http.target", out var value) && value.ToString().Equals(SearchData.Endpoint, StringComparison.OrdinalIgnoreCase));
        if (data != null && data.Latencies.Any())
        {
            int p95Index = GetIndex(data.P95 ?? 0, data.Latencies),
                currentIndex = GetIndex(currentSpan?.Duration ?? 0, data.Latencies);

            var list = data.Latencies?.Select(item => Convert.ToDouble(item.X)).Select(item => Math.Abs(item - data.P95.Value)).ToList();

            timeTypeCount.Data = ConvertDistributionChartData(data.Latencies, currentIndex, p95Index).Json;
            timeTypeCount.HasChart = true;
        }
        else
        {
            timeTypeCount.HasChart = false;
        }
        timeTypeCount.ChartLoading = false;
    }

    private int GetIndex(long current, IEnumerable<ChartPointDto> data)
    {
        if (data == null || !data.Any())
            return 0;

        int index = 0, findIndex = 0;
        double min = current;
        foreach (var item in data)
        {
            var timeKey = Convert.ToDouble(item.X);
            if (latencies.ContainsKey(timeKey))
                latencies[timeKey] += Convert.ToInt32(item.Y);
            else
                latencies.Add(timeKey, Convert.ToInt32(item.Y));
            var value = Convert.ToDouble(item.X);
            var lastMin = Math.Abs(value - current);
            if (lastMin - min < 0)
            {
                findIndex = index;
                min = lastMin;
            }
            index++;
        }
        return findIndex;
    }
    private List<string[]> chartData = new();
    private EChartType ConvertDistributionChartData(IEnumerable<ChartPointDto> data, int current, int p95)
    {
        var chart = EChartConst.Line;
        var list = data.Select(item => new[] { Convert.ToDouble(item.X).FormatTime(), item.Y }).ToList();
        string currentItem = list[current][0], p95Item = list[p95][0];
        chartData = list.GroupBy(item => item[0]).Select(item => new[] { item.Key, item.Sum(values => Convert.ToInt32(values[1])).ToString() }).ToList();

        bool isFoundCurrent = false, isFoundP95 = false;
        var index = chartData.Count - 1;
        do
        {
            if (!isFoundCurrent && chartData[index][0] == currentItem)
            {
                current = index;
                isFoundCurrent = true;
            }
            if (!isFoundP95 && chartData[index][0] == p95Item)
            {
                p95 = index;
                isFoundP95 = true;
            }
            if (isFoundCurrent && isFoundP95)
                break;
            index--;
        } while (index >= 0);

        chart.SetValue("tooltip", new { trigger = "axis", feature = new { dataZoom = new { yAxisIndex = false }, brush = new { type = new string[] { "lineX", "clear" } } } });
        chart.SetValue("toolbox", new { top = -6, show = true });
        chart.SetValue("grid.top", 40);
        chart.SetValue("xAxis", new { type = "category", boundaryGap = false });
        chart.SetValue("yAxis", new { type = "value", boundaryGap = false });
        chart.SetValue("series[0]", new { type = "line", step = "end", symbol = "none", areaStyle = new { } });
        chart.SetValue("series[0].markLine", new { symbol = new string[] { "none", "none" }, lineStyle = new { type = "solid", width = 2 }, label = new { show = true } });
        chart.SetValue("series[0].markLine.data[1]", new { xAxis = p95, lineStyle = new { color = "" }, label = new { formatter = "P95" } });
        chart.SetValue("series[0].markLine.data[0]", new { xAxis = current, lineStyle = new { color = "gray" }, label = new { formatter = "Current" } });
        chart.SetValue("series[0].data", chartData);
        chart.SetValue("brush", new { xAxisIndex = "all", brushLink = "all", outOfBrush = new { colorAlpha = 0.1 }, toolbox = new string[] { "lineX", "clear" }, throttleType = "debounce", throttleDelay = 200, brushStyle = new { color = "#FFA726", borderColor = "#FFA726" } });
        return chart;
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (echartEventModule != null)
            await echartEventModule.DisposeAsync();
        await base.DisposeAsyncCore();
    }
}

public class EChartBrushEventArg
{
    public EChartBrushEventArg() { }

    public EChartBrushEventArg(bool isClear)
    {
        IsClear = isClear;
    }

    public bool IsClear { get; set; }

    public int Start { get; set; }

    public int End { get; set; }
}