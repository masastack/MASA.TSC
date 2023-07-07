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

    private DateTime StartTime;
    private DateTime EndTime;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _options.SetValue("grid", new
        {
            x = 45,
            x2 = 8,
            y2 = 40,
            y = 8
        });
        _options.SetValue("legend.top", "bottom");
        _options.SetValue("legend.textStyle", new { color = "#7681AB" });
        _options.SetValue("tooltip", new
        {
            trigger = "axis",
            axisPointer = new
            {
                type = "cross"
            },
            formatter = @$"function(params){{var html='';for(var i=0;i<params.length;i++){{html+=`<div style='display:flex;justify-content: space-between;padding-bottom:8px;'><div style='display:flex;align-items:center;'><div style='width:8px;height:8px;background-color:${{params[i].color}};border0radius:1px'></div><div>&nbsp;${{params[i].seriesName}}</div></div><div>${{params[i].value}}</div></div>`}}return`<div style='width:136px;margin:-2px'>${{html}}<div style='border-top:1px solid #E4E4E6;margin:0 -8px;'></div><div style='text-align:right;padding-top:2px;'>${{params[0].name}}</div></div>`}}"
        });
        _options.SetValue("xAxis.axisLabel.textStyle", new { color = "#A3AED0" });
        _options.SetValue("xAxis.axisLine.lineStyle", new
        {
            color = "#FF7D00",
            type = "sloid",
            width = 1
        });
        _options.SetValue("yAxis.splitLine.lineStyle", new
        {
            color = "#E4E8F3",
            type = "dashed",
            width = 1
        });
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (!CheckKeyChanged(query))
            return;

        StartTime = query.Start!.Value;
        EndTime = query.End!.Value;
        var step = StartTime.Interval(EndTime);       
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"round(histogram_quantile(0.50,sum(increase(http_server_duration_bucket{{{MetricEnv}}}[{MetricConstants.TIME_PERIOD}])) by (le)),0.01)",
                $"round(histogram_quantile(0.75,sum(increase(http_server_duration_bucket{{{MetricEnv}}}[{MetricConstants.TIME_PERIOD}])) by (le)),0.01)",
                $"round(histogram_quantile(0.90,sum(increase(http_server_duration_bucket{{{MetricEnv}}}[{MetricConstants.TIME_PERIOD}])) by (le)),0.01)",
                $"round(histogram_quantile(0.95,sum(increase(http_server_duration_bucket{{{MetricEnv}}}[{MetricConstants.TIME_PERIOD}])) by (le)),0.01)",
                $"round(histogram_quantile(0.99,sum(increase(http_server_duration_bucket{{{MetricEnv}}}[{MetricConstants.TIME_PERIOD}])) by (le)),0.01)"
            },
            Start = StartTime,
            Service = query.AppId,
            End = EndTime,
            Step = step
        });
        SetData();
    }

    private void SetData()
    {
        Dictionary<string, List<string>> dddd = new();
        var timeSpans = new List<double>();
        if (_data == null)
            return;
        var legend = new string[] { "P50", "P75", "P90", "P95", "P99" };
        var index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                timeSpans.AddRange(((QueryResultMatrixRangeResponse)item.Result[0]).Values!.Select(values => Convert.ToDouble(values[0])));
            }
        }

        timeSpans = timeSpans.Distinct().ToList();
        timeSpans.Sort();
        index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                var key = legend[index++];
                dddd[key] = ((QueryResultMatrixRangeResponse)item.Result[0]).Values!.Select(values => values[1].ToString()).ToList()!;
            }
        }
        _options.SetValue("legend.data", legend.Select(name => new { name, icon = "square" }));
        var format = StartTime.Format(EndTime);
        _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value, format)));
        _options.SetValue("series", dddd.Select(item => new { name = item.Key, type = "line", showSymbol = false, smooth = true, lineStyle = new { type = "solid", width = 4 }, data = item.Value }));
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        SetData();
        StateHasChanged();
    }
}