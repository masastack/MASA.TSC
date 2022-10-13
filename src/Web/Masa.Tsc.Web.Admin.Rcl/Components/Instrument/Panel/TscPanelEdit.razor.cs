// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscPanelEdit
{
    [Parameter]
    public PanelDto Item { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private string _style = "";

    protected override void OnParametersSet()
    {
        StringBuilder text = new();
        if (!ReadOnly)
            text.Append("resize:both;");
        if (Item != null)
        {
            if (!string.IsNullOrEmpty(Item.Height))
                text.Append($"height:{Item.Height}");

            if (!string.IsNullOrEmpty(Item.Width))
                text.Append($"width:{Item.Width}");
        }
        _style = text.ToString();
        base.OnParametersSet();
    }

    private async Task OnDelete()
    {
        if (!await PopupService.ConfirmAsync("删除", "确认要移除该panel吗"))
            return;
        await ApiCaller.PanelService.DeleteAsync(CurrentUserId, Item.InstrumentId, Item.Id);
        await CallParent(OperateCommand.Remove, Item);
    }

    private async Task OnEdit()
    {
        await CallParent(OperateCommand.Update, Item);
    }
}
