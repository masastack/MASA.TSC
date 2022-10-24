// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class EChartPanelDto : PanelDto
{
    public string ChartType { get; set; }

    public override PanelTypes Type => PanelTypes.Chart;

    public List<PanelMetricDto> Metrics { get; set; }
}