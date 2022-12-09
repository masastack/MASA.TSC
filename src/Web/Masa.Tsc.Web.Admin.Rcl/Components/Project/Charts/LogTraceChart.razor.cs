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
    public bool Log { get; set; }

    [Parameter]
    public bool Trace { get; set; }

    [Parameter]
    public string Title { get; set; }

    private EChartType _options = EChartConst.Line;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query == null)
            return;
        DateTime start = DateTime.Now.Date;
        DateTime end = DateTime.Now;
        if (query.Start.HasValue)
            start = query.Start.Value;
        if (query.End.HasValue)
            end = query.End.Value;
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
