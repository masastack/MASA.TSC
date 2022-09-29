// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

/// <summary>
///dialog弹窗回调控制相关
/// </summary>
public partial class TscComponentBase
{
    [Parameter]
    public EventCallback<object[]> OnCallParent { get; set; }

    protected bool _showDialog = false;

    protected async Task CallParent(params object[] values)
    {
        if (OnCallParent.HasDelegate)
            await OnCallParent.InvokeAsync(values);
    }

    protected virtual async Task ChildCallHandler(params object[] values)
    {
        if (values == null || !values.Any())
            return;

        if (values[0] is not OperateCommand command)
            return;

        await ExecuteCommondAsync(command, values[1..]);
        await CallParent(values);
    }

    protected virtual async Task ExecuteCommondAsync(OperateCommand command, object[] values)
    {
        if (command == OperateCommand.Close)
        {
            CloseDialog();
        }
        else if (command == OperateCommand.Open)
        {
            OpenDialog();
        }
        await Task.CompletedTask;
    }

    protected void CloseDialog() => _showDialog = false;
    protected void OpenDialog() => _showDialog = true;
}
