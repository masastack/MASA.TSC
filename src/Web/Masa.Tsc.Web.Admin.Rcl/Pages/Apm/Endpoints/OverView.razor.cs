// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Endpoints;

public partial class OverView
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    private static List<(MetricTypes, string)> metricTypes = new() { (MetricTypes.Avg, "avg"), (MetricTypes.P95, "p95"), (MetricTypes.P99, "p99") };
    private LatencyTypeChartData metricTypeChartData = new();
    private ChartData throughput = new(), failed = new(), timeTypeCount = new();
    private string? lastKey = null;
    private ApmTraceLatencyRequestDto query = new();
    private List<TraceResponseDto>? traceDetails = null;
    private List<ChartPointDto>? errors = null;
    int page = 1, total = 1;
    Dictionary<double, int> latencies = new();
    double percentile = 0;
    //string? traceId = default;    

    protected override async Task OnParametersSetAsync()
    {
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key)
        {
            lastKey = key;
            await LoadDataAsync();
            await LoadDistributionDataAsync();
        }
        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queries = HttpUtility.ParseQueryString(uri.Query);
        var endpoint = queries.Get("endpoint");
        SearchData.Endpoint = endpoint;
        query.Endpoint = endpoint!;
        //traceId = queries.Get("traceId");
        base.OnInitialized();
    }

    private async Task LoadTraceDetailAsync(int page = 1)
    {
        this.page = page;
        string traceId = null;
        var queryDto = new RequestTraceListDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Endpoint = SearchData.Endpoint!,
            Service = SearchData.Service!,
            Env = SearchData.Environment!,
            Page = this.page,
            PageSize = 1,
            Keyword = SearchData.Text
        };
        var result = await ApiCaller.TraceService.GetListAsync(queryDto);
        total = (int)result.Total;
        traceDetails = null;
        errors = null;
        percentile = 0;
        if (result.Result != null && result.Result.Any())
            traceId = result.Result[0].TraceId;

        if (!string.IsNullOrEmpty(traceId))
        {
            traceDetails = (await ApiCaller.TraceService.GetAsync(traceId))?.ToList()!;
            await LoadTraceErrorsAsync(traceId);
            CaculatePercentil();
        }
    }

    private async Task LoadTraceErrorsAsync(string traceId)
    {
        var query = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Env = SearchData.Environment,
            Queries = $"TraceId='{traceId}'",
            Page = 1,
            PageSize = 100
        };
        errors = await ApiCaller.ApmService.GetSpanErrorsAsync(query);
    }

    private void CaculatePercentil()
    {
        if (traceDetails == null || !traceDetails.Any())
            return;
        var current = traceDetails.FirstOrDefault(item => item.Kind == "SPAN_KIND_SERVER"
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

    private async Task LoadDataAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Service = SearchData.Service,
            Env = SearchData.Environment,
            Endpoint = SearchData.Endpoint!,
            Queries = SearchData.Text,
            ComparisonType = SearchData.ComparisonType.ToComparisonType()
        };
        var data = await ApiCaller.ApmService.GetChartsAsync(query);
        if (data != null && data.Any())
        {
            var chartData = data[0];
            {
                metricTypeChartData.Avg = new();
                metricTypeChartData.P95 = new();
                metricTypeChartData.P99 = new();
                throughput = new();
                failed = new();

                metricTypeChartData.Avg.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Latency, unit: "ms", lineName: "Average").Json;
                metricTypeChartData.P95.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.P95, unit: "ms", lineName: "95th percentile").Json;
                metricTypeChartData.P99.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.P99, unit: "ms", lineName: "99th percentile").Json;
                throughput.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Throughput, unit: "tpm").Json;
                failed.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime(CurrentTimeZone).ToString("yyyy/MM/dd HH:mm:ss"), item => item.Failed, unit: "%").Json;

                metricTypeChartData.Avg.ChartLoading = false;
                metricTypeChartData.P95.ChartLoading = false;
                metricTypeChartData.P99.ChartLoading = false;
                throughput.ChartLoading = false;
                failed.ChartLoading = false;
            }
        }
        else
        {
            metricTypeChartData.Avg.EmptyChart = true;
            metricTypeChartData.P95.EmptyChart = true;
            metricTypeChartData.P99.EmptyChart = true;
            throughput.EmptyChart = true;
            failed.EmptyChart = true;
        }
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
        timeTypeCount.ChartLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Service = SearchData.Service,
            Env = SearchData.Environment,
            Endpoint = SearchData.Endpoint!,
            Queries = SearchData.Text,
            ComparisonType = SearchData.ComparisonType.ToComparisonType()
        };
        var data = await ApiCaller.ApmService.GetLatencyDistributionAsync(query);
        await LoadTraceDetailAsync();
        var currentSpan = traceDetails?.Find(item => item.Attributes.TryGetValue("http.target", out var value) && value.ToString().Equals(SearchData.Endpoint, StringComparison.OrdinalIgnoreCase));
        if (data != null)
        {
            int p95Index = GetIndex(data.P95 ?? 0, data.Latencies),
                currentIndex = GetIndex(currentSpan?.Duration ?? 0, data.Latencies);

            var list = data.Latencies?.Select(item => Convert.ToDouble(item.X)).Select(item => Math.Abs(item - data.P95.Value)).ToList();

            timeTypeCount.Data = ConvertDistributionChartData(data.Latencies, currentIndex, p95Index).Json;
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

    private static EChartType ConvertDistributionChartData(IEnumerable<ChartPointDto> data, int current, int p95)
    {
        var chart = EChartConst.Line;
        var list = data.Select(item => new[] { Convert.ToDouble(item.X).FormatTime(), item.Y }).ToList();
        string currentItem = list[current][0], p95Item = list[p95][0];
        var array = list.GroupBy(item => item[0]).Select(item => new[] { item.Key, item.Sum(values => Convert.ToInt32(values[1])).ToString() }).ToList();

        bool isFoundCurrent = false, isFoundP95 = false;
        var index = array.Count - 1;
        do
        {
            if (!isFoundCurrent && array[index][0] == currentItem)
            {
                current = index;
                isFoundCurrent = true;
            }
            if (!isFoundP95 && array[index][0] == p95Item)
            {
                p95 = index;
                isFoundP95 = true;
            }
            if (isFoundCurrent && isFoundCurrent)
                break;
            index--;
        } while (index >= 0);

        chart.SetValue("tooltip", new { trigger = "axis" });
        chart.SetValue("xAxis", new { type = "category", boundaryGap = false });
        chart.SetValue("yAxis", new { type = "value", boundaryGap = false });
        chart.SetValue("series[0]", new { type = "line", step = "end", symbol = "none", areaStyle = new { } });
        chart.SetValue("series[0].markLine", new { symbol = new string[] { "none", "none" }, lineStyle = new { type = "solid", width = 2 }, label = new { show = true } });
        chart.SetValue("series[0].markLine.data[1]", new { xAxis = p95, lineStyle = new { color = "" }, label = new { formatter = "P95" } });
        chart.SetValue("series[0].markLine.data[0]", new { xAxis = current, lineStyle = new { color = "gray" }, label = new { formatter = "Current" } });
        chart.SetValue("series[0].data", array);
        return chart;
    }
}