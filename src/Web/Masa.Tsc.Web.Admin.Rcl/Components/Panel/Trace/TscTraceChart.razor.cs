// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceChart
{
    [Parameter]
    public EventCallback<(DateTime, DateTime)> OnDateTimeRangeUpdate { get; set; }

    [Parameter]
    public ValueTuple<string, string, string>[] Data { get; set; } = Array.Empty<(string, string, string)>();

    [Parameter]
    public bool PageMode { get; set; }

    [Parameter]
    public int Height { get; set; }

    [Parameter]
    public int Width { get; set; }

    private static readonly QuickRangeKey s_defaultQuickRange = QuickRangeKey.Last1Hour;

    private object _option;

    MECharts? MECharts { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _option = GenOption();
        if (MECharts is not null && (Width, Height) != (0, 0))
        {
            await MECharts.Resize(Width, 300);
        }
    }

    private object GenOption()
    {
        if (Data == null)
            return new object();

        return new
        {
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
                data = new[] { "span", "duration" },
                bottom = true
            },
            xAxis = new[]
            {
                new
                {
                    type = "category",
                    data = Data.Select(item=>item.Item1),
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
                    name = "span",
                    axisLabel = new
                    {
                        formatter = "{value}"
                    }
                },
                new
                {
                    type = "value",
                    name = "duration",
                    axisLabel = new
                    {
                        formatter = "{value} ms"
                    }
                },
            },
            series = new[]
            {
                new
                {
                    name = "span",
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
                    name = "duration",
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
                y = 10,
                y2 = 50
            }
        };
    }

    private async Task OnDateTimeUpdate((DateTimeOffset start, DateTimeOffset end) range)
    {
        var localStart = new DateTime(range.start.UtcTicks + range.start.Offset.Ticks, DateTimeKind.Local);
        var localEnd = new DateTime(range.end.UtcTicks + range.end.Offset.Ticks, DateTimeKind.Local);

        await OnDateTimeRangeUpdate.InvokeAsync((localStart, localEnd));
    }

    private async Task OnDateTimeAutoUpdate((DateTimeOffset start, DateTimeOffset end) range)
    {
        var localStart = new DateTime(range.start.UtcTicks + range.start.Offset.Ticks, DateTimeKind.Local);
        var localEnd = new DateTime(range.end.UtcTicks + range.end.Offset.Ticks, DateTimeKind.Local);
        await base.InvokeAsync(async () => await OnDateTimeRangeUpdate.InvokeAsync((localStart, localEnd)));
    }
}