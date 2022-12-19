// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Modules.Echart;

public partial class EChartPanelMetrics
{
    [Parameter]
    public List<PanelMetricDto> Items { get; set; } = new();

    [Parameter]
    public EventCallback<List<PanelMetricDto>> ItemsChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private void Add()
    {
        Items.Add(new PanelMetricDto { });
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, object[] values)
    {
        if (command == OperateCommand.Remove)
        {
            var item = (PanelMetricDto)values[0];
            Items.Remove(item);
            StateHasChanged();
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
}
