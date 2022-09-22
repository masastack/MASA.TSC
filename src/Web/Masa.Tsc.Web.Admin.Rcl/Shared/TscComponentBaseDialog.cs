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
        if (values != null && values.Length >= 1 && values[0] is string str)
        {
            switch (str)
            {
                case "close":
                    _showDialog = false;
                    break;
            }
        }

        await Task.CompletedTask;
    }

    protected void CloseDialog() => _showDialog = false;
    protected void OpenDialog() => _showDialog = true;
}
