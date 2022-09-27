// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class ChartPanelDto : PanelDto
{
    public string ChartType { get; set; }

    public List<PanelMetricDto> Metrics { get; set; }
}