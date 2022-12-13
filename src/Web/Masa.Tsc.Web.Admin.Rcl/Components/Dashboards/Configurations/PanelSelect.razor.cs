﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelSelect
{
    [Parameter]
    public UpsertPanelDto Panel { get; set; }

    [Parameter]
    public EventCallback<UpsertPanelDto> OnSelect { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    List<PanelTypes> GetPanelTypes(PanelTypes type = default)
    {
        if (type == default)
        {
            return new List<PanelTypes>
            {
                PanelTypes.Tabs,
                PanelTypes.Text,
                PanelTypes.Chart,
                PanelTypes.Topology,
                PanelTypes.Log,
                PanelTypes.Trace,
                PanelTypes.Table
            };
        }
        return new List<PanelTypes>();
    }

    async Task SelectPanelAsync(UpsertPanelDto panel, PanelTypes type)
    {
        switch (type)
        {
            case PanelTypes.Tabs:
                panel.ChildPanels = new List<UpsertPanelDto>
                {
                    new()
                    {
                        PanelType = PanelTypes.TabItem,
                        Title = "item1",
                        ChildPanels = new List<UpsertPanelDto>
                        {
                            new()
                        },
                        ParentPanel = panel,
                    }
                };
                break;
            default:
                break;
        }
        panel.PanelType = type;

        await OnSelect.InvokeAsync(panel);
    }
}
