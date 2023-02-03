// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class LogTraceStatiscChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public bool Log { get; set; }

    [Parameter]
    public bool Trace { get; set; }

    [Parameter]
    public bool Error { get; set; }

    [Parameter]
    public bool Warn { get; set; }

    private EChartType _options = EChartConst.Bar;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {

        await Task.CompletedTask;
    }

    private string GetFormat()
    {
        return "M-d";
        //var minites = (int)Math.Round((Query.End - Query.Start).TotalMinutes, 0);
        //if (minites - 20 <= 0)
        //    return "HH:mm";
        //if (minites - 100 <= 0)
        //    return "HH:mm";
        //if (minites - 210 <= 0)
        //    return "HH:mm";
        //if (minites - 600 <= 0)
        //    return "HH:mm";

        //var hours = minites / 60;
        //if (hours - 20 <= 0)
        //    return "dd H";
        //if (hours - 60 <= 0)
        //    return "dd H";
        //if (hours - 120 <= 0)
        //    return "dd H";
        //if (hours - 240 <= 0)
        //    return "dd H";

        //var days = hours / 24;
        //if (days - 20 <= 0)
        //    return "MM-dd";

        //return "yy-MM";
    }
}
