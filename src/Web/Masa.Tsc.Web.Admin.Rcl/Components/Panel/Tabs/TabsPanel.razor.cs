// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Tabs;

public partial class TabsPanel
{
    [Parameter]
    public UpsertTabsPanelDto Panel { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    [CascadingParameter]
    public List<PanelGrids> PanelGridRange { get; set; }

    [Parameter]
    public bool IsEditTabItem { get; set; }

    [DisallowNull]
    StringNumber? CurrentTab
    {
        get => Panel.CurrentTabItem?.Id.ToString();
        set 
        {
            Panel.SetCurrentTabItem(Guid.Parse(value.ToString()));
            if(Panel.CurrentTabItem is not null)
            {
                foreach(var child in Panel.CurrentTabItem.ChildPanels)
                {
                    if(child is UpsertChartPanelDto chartPanel)
                    {
                        chartPanel.SetChartKey("tabItem");
                    }
                }
            }
        }
    }

    protected override void OnParametersSet()
    {
        if (Panel.PanelType != PanelTypes.Tabs || Panel.ChildPanels.Any(child => child.PanelType != PanelTypes.TabItem))
            OpenErrorMessage("Invalid tabs panel");
    }

    void CloseTabItem(UpsertPanelDto panel)
    {
        RemovePanelGrid(panel);
        Panel.RemoveTabItem(panel);
    }

    void RemovePanelGrid(UpsertPanelDto panel)
    {
        PanelGridRange.RemoveAll(item => item.ParentPanel == panel);
        if (panel.ChildPanels.Any())
        {
            //PanelGridRange.RemoveAll(item => item.Panels.Any(item2 => item2.ParentPanel?.Id == panel.Id));
            foreach (var item in panel.ChildPanels)
            {
                RemovePanelGrid(item);
            }
        }
    }
}
