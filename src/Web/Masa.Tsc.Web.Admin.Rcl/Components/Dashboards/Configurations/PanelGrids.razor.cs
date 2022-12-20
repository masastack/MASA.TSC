﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Components.Panel.Tabs.Models;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelGrids
{
    [Parameter]
    public List<UpsertPanelDto> Panels { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    protected override void OnInitialized()
    {
        IniPanels(Panels);
    }

    void IniPanels(List<UpsertPanelDto> panels, UpsertPanelDto? parentPanel = null)
    {
        foreach (var panel in panels)
        {
            panel.ParentPanel = parentPanel;
            if(panel.ChildPanels.Any())
            {
                IniPanels(panel.ChildPanels, panel);
            }           
        }
    }

    void AddChildPanel(UpsertTabsPanelDto? panel)
    {
        panel?.CurrentTabItem?.AddPanel();
        //if (panel.PanelType is PanelTypes.Tabs)
        //{
        //    panel.CurrentTabItem!.ChildPanels.Add(new UpsertPanelDto()
        //    {
        //        ParentPanel = panel.CurrentTabItem
        //    });
        //}
        //else
        //{
        //    panel.ChildPanels.Add(new() 
        //    {
        //        ParentPanel = panel
        //    });
        //}
    }

    void AddTabItem(UpsertTabsPanelDto? panel)
    {
        panel?.AddTabItem();
        //var tabItem = new UpsertPanelDto
        //{
        //    PanelType = PanelTypes.TabItem,
        //    Title = $"item {panel.ChildPanels.Count + 1}",
        //    ParentPanel = panel,
        //};
        //tabItem.ChildPanels = new List<UpsertPanelDto>
        //{
        //    new()
        //    {
        //         ParentPanel = tabItem
        //    }
        //};
        //panel.ChildPanels.Add(tabItem);
        //panel.CurrentTabItem = tabItem;
    }

    void RemovePanel(UpsertPanelDto panel)
    {
        if(panel is UpsertTabItemPanelDto tabItem)
        {
            tabItem.RemovePanel(panel);
        }
        else if (panel.ParentPanel is null)
            Panels.Remove(panel);
        else
            panel.ParentPanel.ChildPanels.Remove(panel);
    }

    void ReplacePanel(UpsertPanelDto panel)
    {
        Panels.RemoveAll(p => p.Id == panel.Id);
        Panels.Add(panel);
    }
}
