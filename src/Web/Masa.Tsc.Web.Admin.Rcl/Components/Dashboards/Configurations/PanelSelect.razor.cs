// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelSelect
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public Guid PanelId { get; set; }

    [Parameter]
    public EventCallback<UpsertPanelDto> OnSelect { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    List<KeyValuePair<PanelTypes, string>> GetPanelTypes(PanelTypes type = default)
    {
        var types = new List<KeyValuePair<PanelTypes, string>>
        {
            new KeyValuePair<PanelTypes, string>(PanelTypes.Tabs,"mdi-tab"),
            new KeyValuePair<PanelTypes, string>(PanelTypes.Text,"mdi-format-size"),
            new KeyValuePair<PanelTypes, string>(PanelTypes.Chart,"mdi-chart-box"),
            new KeyValuePair<PanelTypes, string>(PanelTypes.Topology,"mdi-sitemap"),
            new KeyValuePair<PanelTypes, string>(PanelTypes.Log,"fas fa-list"),
            new KeyValuePair<PanelTypes, string>(PanelTypes.Trace,"fas fa-eye"),
            //PanelTypes.Table
        };
        return types;
    }

    async Task SelectPanelAsync(PanelTypes type)
    {
        UpsertPanelDto panel = new();
        switch (type)
        {
            case PanelTypes.Tabs:
                panel = new UpsertTabsPanelDto(PanelId);
                break;
            case PanelTypes.Chart or PanelTypes.Table:
                panel = new UpsertChartPanelDto(PanelId);
                break;
            default:
                panel.Id = PanelId;
                panel.PanelType = type;
                break;
        }

        await OnSelect.InvokeAsync(panel);
    }
}
