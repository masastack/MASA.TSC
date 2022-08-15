// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartLineOption
{
    public EChartPieOptionTitle Title { get; set; }

    public object Tooltip => new { Trigger = "axis" };

    public EChartOptionLegend Legend { get; set; }

    public EChartOptionGrid Grid { get; set; }

    public EChartOptionAxis XAxis { get; set; } = new EChartOptionAxis()
    {
        Type = "category",
        BoundaryGap = false,
        AxisLine = new()
        {
            Show = true
        }
    };

    public EChartOptionAxis YAxis { get; set; } = new EChartOptionAxis()
    {
        Type = "value",
        AxisLine = new()
        {
            Show = true
        }
    };

    public EChartLineOptionSerie[] Series { get; set; }
}
