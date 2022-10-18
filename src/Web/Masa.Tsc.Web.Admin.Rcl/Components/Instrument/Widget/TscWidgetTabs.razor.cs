// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetTabs
{
    private StringNumber _value;
    private TabsPanelDto _panelValue = new() { Title = "tabs" };

    private async void AddTab()
    {
        var add = new TabItemPanelDto { Title = "tabnew", Id = Guid.NewGuid(), InstrumentId = _panelValue.InstrumentId, Sort = _panelValue.Tabs.Count + 1, ParentId = _panelValue.ParentId, Type = PanelTypes.TabItem };
        await ApiCaller.PanelService.AddAsync(add);
        _panelValue.Tabs.Add(add);
    }

    public override PanelDto Value
    {
        get => _panelValue;
        set
        {
            if (value is null) _panelValue = new()
            {
                Title = "tabs"
            };
            else if (value is TabsPanelDto dto) _panelValue = dto;
            else
            {
                _panelValue.Id = value.Id;
                _panelValue.ParentId = value.ParentId;
                _panelValue.InstrumentId = value.InstrumentId;
            }
            if (_panelValue.Tabs == null || !_panelValue.Tabs.Any())
            {
                _panelValue.Tabs = new List<TabItemPanelDto>
                    {
                        new TabItemPanelDto{ Id=Guid.NewGuid(),Title="tab1",InstrumentId=_panelValue.InstrumentId,Sort=1 },
                        new TabItemPanelDto{ Id=Guid.NewGuid(),Title="tab2",InstrumentId=_panelValue.InstrumentId,Sort=2 },
                        new TabItemPanelDto{Id=Guid.NewGuid(),Title="tab3",InstrumentId=_panelValue.InstrumentId,Sort=3 }
                    };
            }
            _value = _panelValue.Tabs[0].Id.ToString();
        }
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, object[] values)
    {
        if (command == OperateCommand.Remove)
        {
            if (values != null && values.Length > 0 && values[0] is string id)
            {
                var guid = Guid.Parse(id);
                var item = _panelValue.Tabs.FirstOrDefault(t => t.Id == guid);
                if (item != null)
                    _panelValue.Tabs.Remove(item);
            }
        }

        await Task.CompletedTask;
        return false;
    }
}