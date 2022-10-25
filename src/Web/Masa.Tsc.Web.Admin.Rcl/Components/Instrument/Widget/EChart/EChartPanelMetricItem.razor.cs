// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class EChartPanelMetricItem
{
    [Parameter]
    public EventCallback<EChartPanelMetricItemModel> ValueChanged { get; set; }

    [Parameter]
    public EChartPanelMetricItemModel Value { get; set; }

    [Parameter]
    public List<string> Names { get; set; }    

    private async Task OnValueChanged()
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }    

    private async Task OnDeleteAsync()
    {
        await OnCallParent(OperateCommand.Remove, Value);
    }
}