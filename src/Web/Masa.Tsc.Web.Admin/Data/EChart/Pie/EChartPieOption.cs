// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartPieOption
{
    public EChartPieOptionTitle Title { get; set; }

    public object Tooltip => new { Trigger = "item" };

    public EChartOptionLegend Legend { get; set; }

    public EChartOptionSerie[] Series { get; set; }
}
