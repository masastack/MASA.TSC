// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelSelect
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertPanelDto Panel { get; set; }

    [Parameter]
    public EventCallback<UpsertPanelDto> OnSelect { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    List<(PanelTypes PannelType, string Icon, bool Disabled)> GetPanelTypes(PanelTypes type = default)
    {
        var types = new List<(PanelTypes PannelType, string Icon, bool Disabled)>
        {
            new (PanelTypes.Text,"mdi-format-size", false),
            new (PanelTypes.Chart,"mdi-chart-box", false),
            new (PanelTypes.Topology,"mdi-sitemap", false),
            new (PanelTypes.Log,"fas fa-list", false),
            new (PanelTypes.Trace,"fas fa-eye", false),
        };
        if (Panel.ParentPanel == null || Panel.ParentPanel.PanelType != PanelTypes.TabItem || Panel.ParentPanel.ParentPanel == null || Panel.ParentPanel.ParentPanel.ParentPanel == null)
        {
            types.Insert(0, new(PanelTypes.Tabs, "mdi-tab", false));
        }
        else
        {
            types.Insert(0, new(PanelTypes.Tabs, "mdi-tab", true));
        }
        return types;
    }

    async Task SelectPanelAsync(PanelTypes type)
    {
        UpsertPanelDto panel = new();
        switch (type)
        {
            case PanelTypes.Tabs:
                panel = new UpsertTabsPanelDto(Panel.Id);
                break;
            case PanelTypes.Chart or PanelTypes.Table:
                panel = new UpsertChartPanelDto(Panel.Id);
                break;
            default:
                panel.Id = Panel.Id;
                panel.PanelType = type;
                break;
        }

        await OnSelect.InvokeAsync(panel);
    }
}
