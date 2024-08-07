﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceChart
{
    [Parameter]
    public EventCallback<(DateTime, DateTime)> OnDateTimeRangeUpdate { get; set; }

    [Parameter]
    public ValueTuple<long, string, string>[] Data { get; set; } = Array.Empty<(long, string, string)>();

    [Parameter]
    public double Height { get; set; }

    [Parameter]
    public double Width { get; set; }

    [Parameter]
    public DateTime Start { get; set; }

    [Parameter]
    public DateTime End { get; set; }

    MECharts? MECharts { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (MECharts is not null && (Width, Height) != (0, 0))
        {
            await MECharts.Resize(Width, 300);
        }
    }

    protected async Task ResizeEChartAsync()
    {
        if (MECharts is not null)
            await MECharts.Resize();
    }

    private object GenOption()
    {
        if (Data == null)
            return new object();

        var _chartFormat = Start.Format(End);
        var subText = $"{Start.UtcFormatLocal(CurrentTimeZone)}～{End.UtcFormatLocal(CurrentTimeZone)}";
        var names = new string[] { I18n.Trace("Span Count"), I18n.Trace("Duration") + "("+ I18n.T("ms") + ")" };

        return new
        {
            title = new
            {
                right = 50,
                top = -14,
                subtext = subText,
            },
            tooltip = new
            {
                trigger = "axis",
                axisPointer = new
                {
                    type = "cross",
                    crossStyle = new { color = "#A18BFF66" }
                }
            },
            legend = new
            {
                data = names,
                bottom = true
            },
            xAxis = new[]
            {
                new
                {
                    type = "category",
                    data = Data.Select(item=>item.Item1.ToDateTime(CurrentTimeZone).Format(_chartFormat)),
                    axisPointer = new
                    {
                        type = "shadow"
                    }
                },
            },
            yAxis = new[]
            {
                new
                {
                    type = "value",
                    //name = $"span({T("Total Count")})"
                },
                new
                {
                    type = "value",
                    //name = $"duration(ms)"
                },
            },
            series = new[]
            {
                new
                {
                    name =names[0] ,
                    type = "bar",
                    yAxisIndex = 0,
                    data = Data.Select(item=>item.Item2),
                    itemStyle = new
                    {
                        color = "#4318FF"
                    },
                    lineStyle = new
                    {
                        color = "",
                        type = ""
                    },
                    smooth = false
                },
                new
                {
                    name =names[1],
                    type = "line",
                    yAxisIndex = 1,
                    data = Data.Select(item=>item.Item3),
                    itemStyle = new
                    {
                        color = ""
                    },
                    lineStyle = new
                    {
                        color = "#A18BFF",
                        type = "dashed"
                    },
                    smooth = true
                }
            },
            Grid = new
            {
                x = 70,
                x2 = 70,
                y = 20,
                y2 = 50
            }
        };
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        StateHasChanged();
    }
}