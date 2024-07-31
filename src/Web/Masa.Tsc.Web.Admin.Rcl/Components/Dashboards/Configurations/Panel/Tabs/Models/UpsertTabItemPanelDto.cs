// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Tabs.Models;

public class UpsertTabItemPanelDto : UpsertPanelDto
{
    public UpsertTabItemPanelDto(UpsertTabsPanelDto parentPanel, string? title = null)
    {
        PanelType = PanelTypes.TabItem;
        Title = title ?? "item1";
        ChildPanels = new List<UpsertPanelDto>
        {
            new()
            {
                ParentPanel = this,
                Width = 12,
                Height = 5
            }
        };
        ParentPanel = parentPanel;
    }

    public void AddPanel(PanelTypes panelType = default)
    {
        ChildPanels.AdaptiveUI(new UpsertPanelDto()
        {
            PanelType = panelType,
            AutoPosition = true,
            ParentPanel = this
        });
    }

    public void RemovePanel(UpsertPanelDto panel)
    {
        ChildPanels.Remove(panel);
    }
}
