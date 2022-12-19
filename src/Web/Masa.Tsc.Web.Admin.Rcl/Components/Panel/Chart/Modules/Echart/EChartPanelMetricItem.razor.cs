// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Modules.Echart;

public partial class EChartPanelMetricItem
{
    [Parameter]
    public EventCallback<PanelMetricDto> ValueChanged { get; set; }

    [Parameter]
    public PanelMetricDto Value { get; set; }

    private async Task OnDeleteAsync()
    {
        await OnCallParent(OperateCommand.Remove, Value);
    }
}