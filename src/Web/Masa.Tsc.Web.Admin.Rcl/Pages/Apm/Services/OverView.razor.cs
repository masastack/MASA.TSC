// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Services;

public partial class OverView
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    private static List<(MetricTypes, string)> metricTypes = new() { (MetricTypes.Avg, "avg"), (MetricTypes.P95, "p95"), (MetricTypes.P99, "p99") };
    private LatencyTypeChartData metricTypeChartData = new();
    private ChartData throughput = new(), failed = new();
    string lastKey = null;

    private void OnMetricTypeChanged(MetricTypes type)
    {
        metricTypeChartData.MetricType = type;
    }

    protected override async Task OnParametersSetAsync()
    {
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key)
        {
            lastKey = key;
            await LoadDataAsync();
        }
        await base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        await base.OnInitializedAsync();
    }

    private async Task LoadDataAsync()
    {
        var query = new BaseApmRequestDto
        {
            Start = SearchData.Start,
            End = SearchData.End,
            Service = SearchData.Service,
            Env = SearchData.Enviroment,
            ComparisonType = SearchData.ComparisonType.ToComparisonType()
        };
        if (SearchData.ComparisonType == ApmComparisonTypes.Day)
        {
            query.ComparisonType = ComparisonTypes.DayBefore;
        }
        else if (SearchData.ComparisonType == ApmComparisonTypes.Week)
        {
            query.ComparisonType = ComparisonTypes.WeekBefore;
        }
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

                metricTypeChartData.Avg.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss"), item => item.Latency, unit: "ms", lineName: I18n.Apm("Chart.Average")).Json;
                metricTypeChartData.P95.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss"), item => item.P95, unit: "ms", lineName: I18n.Apm("Chart.p95")).Json;
                metricTypeChartData.P99.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss"), item => item.P99, unit: "ms", lineName: I18n.Apm("Chart.p99")).Json;
                throughput.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss"), item => item.Throughput, unit: "tpm").Json;
                failed.Data = ConvertLatencyChartData(chartData, item => item.Time.ToDateTime().ToString("yyyy/MM/dd HH:mm:ss"), item => item.Failed, unit: "%").Json;

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
}
