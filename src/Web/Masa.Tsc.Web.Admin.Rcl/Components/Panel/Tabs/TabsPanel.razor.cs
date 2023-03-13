// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Tabs;

public partial class TabsPanel
{
    [Parameter]
    public UpsertTabsPanelDto Panel { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    [DisallowNull]
    StringNumber? CurrentTab
    {
        get => Panel.CurrentTabItem?.Id.ToString();
        set
        {
            Panel.SetCurrentTabItem(Guid.Parse(value.ToString()!));
        }
    }

    protected override void OnParametersSet()
    {
        if (Panel.PanelType != PanelTypes.Tabs || Panel.ChildPanels.Any(child => child.PanelType != PanelTypes.TabItem))
            OpenErrorMessage("Invalid tabs panel");
    }

    void AddTabItem()
    {
        Panel.AddTabItem();
    }

    void CloseTabItem(UpsertPanelDto panel)
    {       
        Panel.RemoveTabItem(panel);
    }
}
