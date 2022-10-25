// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class EChartPanelMetrics
{
    [Parameter]
    public List<EChartPanelMetricItemModel> Items { get; set; } = new();

    [Parameter]
    public EventCallback<List<EChartPanelMetricItemModel>> ItemsChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private List<string> _names;

    private async Task OnItemsChange()
    {
        if (ItemsChanged.HasDelegate)
            await ItemsChanged.InvokeAsync(Items);
    }

    protected override async Task OnInitializedAsync()
    {
        _names = await ApiCaller.MetricService.GetNamesAsync();
        await base.OnInitializedAsync();
    }

    private void Add()
    {
        Items.Add(new EChartPanelMetricItemModel { });
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, object[] values)
    {
        if (command == OperateCommand.Remove)
        {
            var item = (EChartPanelMetricItemModel)values[0];
            Items.Remove(item);
            StateHasChanged();
            await OnItemsChange();
            return true;
        }
        return false;
    }
}
