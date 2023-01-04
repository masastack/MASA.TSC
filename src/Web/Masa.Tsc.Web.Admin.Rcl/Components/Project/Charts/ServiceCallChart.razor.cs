// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ServiceCallChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private EChartType _options = EChartConst.Line;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query == null)
            return;
        DateTime start = DateTime.Now.AddDays(-1);
        DateTime end = DateTime.Now;
        if (query.Start.HasValue)
            start = query.Start.Value;
        if (query.End.HasValue)
            end = query.End.Value;

        var step = (int)Math.Floor((end - start).TotalSeconds / 250);
        if (step - 5 < 0)
            step = 5;
        var result = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"sum by(service_name)(increase(http_server_duration_count{{service_name=\"{query.AppId}\"}}[1m]))"
                //$"sum by(service_name) (sum_over_time(http_server_duration_bucket{{service_name=\"{query.AppId}\"}}[10h]))/sum by(service_name)(count_over_time(http_server_duration_bucket{{service_name=\"{query.AppId}\"}}[10h]))"
            },
            Start = start,
            End = end,
            Step = step.ToString()
        });

        List<string> values = new();
        var timeSpans = new List<double>();
        
        if (result[0] != null && result[0].ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && result[0].Result != null && result[0].Result.Any())
        {
            timeSpans.AddRange(((QueryResultMatrixRangeResponse)result[0].Result[0]).Values.Select(values => Convert.ToDouble(values[0])));
            values = ((QueryResultMatrixRangeResponse)result[0].Result[0]).Values.Select(values => values[1].ToString()).ToList();
        }
       
        _options.SetValue("xAxis.data", timeSpans.Select(value=>ToDateTimeStr(value)));
        _options.SetValue("series[0].data", values);
    }
}
