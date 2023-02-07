// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ServiceResponseTimePercentile
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private EChartType _options = EChartConst.Line;
    private List<QueryResultDataResponse>? _data;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        DateTime start = query.Start!.Value, end = query.End!.Value;
        var step = start.Interval(end);
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"round(histogram_quantile(0.50,sum(increase(http_server_duration_bucket[5m])) by (le)),0.01)",
                $"round(histogram_quantile(0.75,sum(increase(http_server_duration_bucket[5m])) by (le)),0.01)",
                $"round(histogram_quantile(0.90,sum(increase(http_server_duration_bucket[5m])) by (le)),0.01)",
                $"round(histogram_quantile(0.95,sum(increase(http_server_duration_bucket[5m])) by (le)),0.01)",
                $"round(histogram_quantile(0.99,sum(increase(http_server_duration_bucket[5m])) by (le)),0.01)"
            },
            Start = start,
            ServiceName = query.AppId,
            End = end,
            Step = step
        });

        Dictionary<string, List<string>> dddd = new();
        var timeSpans = new List<double>();

        var legend = new string[] { "P50", "P75", "P90", "P95", "P99" };
        var index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                timeSpans.AddRange(((QueryResultMatrixRangeResponse)item.Result[0]).Values!.Select(values => Convert.ToDouble(values[0])));
            }
        }

        timeSpans = timeSpans.Distinct().ToList();
        timeSpans.Sort();
        index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                var key = legend[index++];
                dddd[key] = ((QueryResultMatrixRangeResponse)item.Result[0]).Values!.Select(values => values[1].ToString()).ToList()!;
            }
        }

        _options.SetValue("grid", new
        {
            x = 60,
            x2 = 20,
            y2 = 20,
            y = 25
        });
        _options.SetValue("legend.data", legend);
        _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value)));
        _options.SetValue("series", dddd.Select(item => new { name = item.Key, type = "line", data = item.Value }));
    }
}