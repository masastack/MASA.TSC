// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Tabs;

public partial class TabsPanel
{
    [Parameter]
    public UpsertPanelDto Panel { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    [DisallowNull]
    StringNumber? CurrentTab
    {
        get => Panel.CurrentTabItem?.Id.ToString();
        set => Panel.CurrentTabItem = Panel.ChildPanels.First(child => child.Id == Guid.Parse(value.ToString()));
    }

    protected override void OnParametersSet()
    {
        if (Panel.PanelType != PanelTypes.Tabs || Panel.ChildPanels.Any(child => child.PanelType != PanelTypes.TabItem))
            OpenErrorMessage("Invalid tabs panel");
    }

    void CloseTabItem(UpsertPanelDto panel)
    {
        var index = panel.ParentPanel!.ChildPanels.IndexOf(panel);
        if (index == 0)
        {
            panel.ParentPanel!.ChildPanels.Remove(panel);
            Panel.CurrentTabItem = panel.ParentPanel.ChildPanels.FirstOrDefault();
        }
        else if(index == panel.ParentPanel!.ChildPanels.Count -1)
        {
            panel.ParentPanel!.ChildPanels.Remove(panel);
            Panel.CurrentTabItem = panel.ParentPanel!.ChildPanels.Last();
        }
        else
        {
            Panel.CurrentTabItem = panel.ParentPanel!.ChildPanels[index + 1];
            panel.ParentPanel!.ChildPanels.Remove(panel);
        }
    }
}
