// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartBarOption
{
    public EChartOptionAxis XAxis { get; set; }

    public EChartOptionAxis YAxis { get; set; }

    public EChartOptionBarSeries[] Series { get; set; }
}
