// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

/// <summary>
///dialog弹窗回调控制相关
/// </summary>
public partial class TscComponentBase
{
    [Parameter]
    public EventCallback<object[]> CallParent { get; set; }

    protected bool _showDialog = false;

    /// <summary>
    /// 传递使用
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    protected async Task OnCallParent(params object[] values)
    {
        if (CallParent.HasDelegate)
            await CallParent.InvokeAsync(values);
    }

    /// <summary>
    /// 当前父组件调用实现
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    protected virtual async Task ChildCallHandler(params object[] values)
    {
        if (values == null || !values.Any())
            return;

        if (values[0] is not OperateCommand command)
            return;

        //await CallParent(values);
        if(!await ExecuteCommondAsync(command, values[1..]))
            await OnCallParent(values);
    }

    protected virtual async Task<bool> ExecuteCommondAsync(OperateCommand command, params object[] values)
    {
        if (command == OperateCommand.Close)
        {
            CloseDialog();
            return true;
        }
        else if (command == OperateCommand.Open)
        {
            OpenDialog();
            return true;
        }
        await Task.CompletedTask;
        return false;
    }

    protected void CloseDialog() => _showDialog = false;
    protected void OpenDialog() => _showDialog = true;
}
