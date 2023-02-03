// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

internal class EChartPieBorderRadiusOption
{
    public static object Tooltip => new
    {
        Trigger = "item"
    };

    public EChartOptionLegend Legend { get; set; }

    public EChartPieBorderRadiusOptionSerie[] Series { get; set; } = new EChartPieBorderRadiusOptionSerie[1];
}
