// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Tabs.Models;

public class UpsertTabsPanelDto : UpsertPanelDto
{
    UpsertTabItemPanelDto? _currentTabItem;

    public UpsertTabItemPanelDto? CurrentTabItem
    {
        get => _currentTabItem ?? ChildPanels.FirstOrDefault() as UpsertTabItemPanelDto;
        set => _currentTabItem = value;
    }

    public UpsertTabsPanelDto(Guid id)
    {
        Id = id;
        PanelType = PanelTypes.Tabs;
        AddTabItem();
    }

    public void AddTabItem()
    {
        var title = $"item {ChildPanels.Count + 1}";
        var tabItem = new UpsertTabItemPanelDto(this, title);
        ChildPanels.Add(tabItem);
        CurrentTabItem = tabItem;
    }

    public void RemoveTabItem(UpsertPanelDto panel)
    {
        var index = ChildPanels.IndexOf(panel);
        if (index == 0)
        {
            ChildPanels.Remove(panel);
            CurrentTabItem = ChildPanels.FirstOrDefault() as UpsertTabItemPanelDto;
        }
        else if (index == panel.ParentPanel!.ChildPanels.Count - 1)
        {
            ChildPanels.Remove(panel);
            CurrentTabItem = ChildPanels.Last() as UpsertTabItemPanelDto;
        }
        else
        {
            CurrentTabItem = ChildPanels[index + 1] as UpsertTabItemPanelDto;
            ChildPanels.Remove(panel);
        }
    }

    public void SetCurrentTabItem(Guid id)
    {
        CurrentTabItem = ChildPanels.First(child => child.Id == id) as UpsertTabItemPanelDto;
    }
}
