// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class LogTraceChart
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
        if (query == null)
            return;
        DateTime start = DateTime.UtcNow.AddDays(-1);
        DateTime end = DateTime.UtcNow;
        if (query.Start.HasValue)
            start = query.Start.Value;
        if (query.End.HasValue)
            end = query.End.Value;

        var step = (int)Math.Floor((end - start).TotalSeconds / 250);
        if (step - 5 < 0)
            step = 5;
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                "histogram_quantile(0.50,sum(increase(http_server_duration_bucket[1m])) by (le))",
                "histogram_quantile(0.75,sum(increase(http_server_duration_bucket[1m])) by (le))",
                "histogram_quantile(0.90,sum(increase(http_server_duration_bucket[1m])) by (le))",
                "histogram_quantile(0.95,sum(increase(http_server_duration_bucket[1m])) by (le))",
                "histogram_quantile(0.99,sum(increase(http_server_duration_bucket[1m])) by (le))"
            },
            Start = start,
            End = end,
            Step = step.ToString()
        });

        Dictionary<string, List<string>> dddd = new Dictionary<string, List<string>>();
        var timeSpans = new List<double>();

        var legend = new string[] {"P50","P75","P90","P95","P99"};
        var index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                var key = legend[index++];
                timeSpans.AddRange(((QueryResultMatrixRangeResponse)item.Result[0]).Values.Select(values => Convert.ToDouble(values[0])));
            }
        }

        timeSpans=timeSpans.Distinct().ToList();
        timeSpans.Sort();
        index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                var key = legend[index++];
                dddd[key]=((QueryResultMatrixRangeResponse)item.Result[0]).Values.Select(values => values[1].ToString()).ToList()!;
            }
        }

        _options.SetValue("grid", new 
        {
            x=60,x2=20
        });
        _options.SetValue("legend.data", legend);
        _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value)));
        _options.SetValue("series", dddd.Select(item => new {name=item.Key,type="line",data=item.Value }));


        await Task.CompletedTask;
    }

    private string GetInterval(DateTime start, DateTime end)
    {
        var minites = (int)Math.Round((end - start).TotalMinutes, 0);
        if (minites - 20 <= 0)
            return "1m";
        if (minites - 100 <= 0)
            return "5m";
        if (minites - 210 <= 0)
            return "15m";
        if (minites - 600 <= 0)
            return "30m";

        var hours = minites / 60;
        if (hours - 20 <= 0)
            return "1h";
        if (hours - 60 <= 0)
            return "3h";
        if (hours - 120 <= 0)
            return "6h";
        if (hours - 240 <= 0)
            return "12h";

        var days = hours / 24;
        if (days - 20 <= 0)
            return "1d";

        return "1month";
    }

    private string GetFormat(DateTime start, DateTime end)
    {
        //return "M-d";
        var minites = (int)Math.Round((end - start).TotalMinutes, 0);
        if (minites - 20 <= 0)
            return "HH:mm";
        if (minites - 100 <= 0)
            return "HH:mm";
        if (minites - 210 <= 0)
            return "HH:mm";
        if (minites - 600 <= 0)
            return "HH:mm";

        var hours = minites / 60;
        if (hours - 20 <= 0)
            return "dd H";
        if (hours - 60 <= 0)
            return "dd H";
        if (hours - 120 <= 0)
            return "dd H";
        if (hours - 240 <= 0)
            return "dd H";

        var days = hours / 24;
        if (days - 20 <= 0)
            return "MM-dd";

        return "yy-MM";
    }

    private Dictionary<string, string> ConvertToLogQueries(ProjectAppSearchModel query)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("service.name", query.AppId);
        return dic;
    }

    private Dictionary<string, string> ConvertToTraceQueries(ProjectAppSearchModel query, bool isSpan = false, bool isTrace = false)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("service.name", query.AppId);

        dic.Add("isTrace", isTrace.ToString().ToLower());
        dic.Add("isSpan", isSpan.ToString().ToLower());

        return dic;
    }
}
