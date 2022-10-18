// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetTabItem
{
    private TabItemPanelDto _panelValue = new() { Title = "tabs" };

    public override PanelDto Value
    {
        get => _panelValue;
        set
        {
            if (value is null) _panelValue = new()
            {
                Title = "tabItem"
            };
            else if (value is TabItemPanelDto dto) _panelValue = dto;
            else
            {
                _panelValue.Id = value.Id;
                _panelValue.ParentId = value.ParentId;
                _panelValue.InstrumentId = value.InstrumentId;
                _panelValue.Sort = value.Sort;
                _panelValue.Title = value.Title;
            }
        }
    }

    private async Task OnNameChange(string value)
    {
        if (_panelValue.Title != value)
        {            
            await ApiCaller.PanelService.UpdateAsync(new UpdatePanelDto
            {
                Id = _panelValue.Id,
                InstrumentId = _panelValue.InstrumentId,
                Name = value,
                Sort = _panelValue.Sort
            });
            _panelValue.Title = value;
        }
    }

    private async Task OnRemove()
    {
        if (!await PopupService.ConfirmAsync("Remove Confirm", "Remove this Tab Item?"))
            return;
        await ApiCaller.PanelService.DeleteAsync(CurrentUserId, _panelValue.InstrumentId, _panelValue.Id);
        await CallParent(OperateCommand.Remove, _panelValue.Id.ToString());
    }
}