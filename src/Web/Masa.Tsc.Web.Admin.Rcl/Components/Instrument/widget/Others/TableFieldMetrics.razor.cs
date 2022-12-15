// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TableFieldMetrics
{
    [Parameter]
    public List<TableFieldItemModel> Items { get; set; } = new();

    [Parameter]
    public EventCallback<List<TableFieldItemModel>> ItemsChanged { get; set; }

    private List<string> _names;

    protected override async Task OnInitializedAsync()
    {
        _names = await ApiCaller.MetricService.GetNamesAsync();
        await base.OnInitializedAsync();
    }

    private void Add()
    {
        Items.Add(new TableFieldItemModel { });
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, object[] values)
    {
        if (command == OperateCommand.Remove)
        {
            var item = (TableFieldItemModel)values[0];
            Items.Remove(item);
            StateHasChanged();
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
}
