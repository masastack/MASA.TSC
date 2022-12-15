// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TableFieldMetricItem
{
    [Parameter]
    public TableFieldItemModel Value { get; set; }

    [Parameter]
    public EventCallback<TableFieldItemModel> ValueChanged { get; set; }

    private async Task OnDeleteAsync()
    {
        await OnCallParent(OperateCommand.Remove, Value);
    }
}