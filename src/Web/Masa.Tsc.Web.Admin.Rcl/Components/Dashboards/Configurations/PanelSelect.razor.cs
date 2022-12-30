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
    public bool IsEdit { get; set; }

    [CascadingParameter]
    public List<PanelGrids> PanelGridRange { get; set; }

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

    async Task SelectPanelAsync(PanelTypes type)
    {
        await Task.WhenAll(PanelGridRange.Select(item => item.SavePanelGridAsync()));
        UpsertPanelDto panel = new();
        switch (type)
        {
            case PanelTypes.Tabs:
                panel = new UpsertTabsPanelDto(PanelId);
                break;
            case PanelTypes.Chart:
                panel = new UpsertChartPanelDto(PanelId);
                NavigationManager.NavigateTo($"/dashboard/configuration/chart/{panel.Id}");
                break;
            default:
                panel.Id = PanelId;
                panel.PanelType = type;
                break;
        }        

        await OnSelect.InvokeAsync(panel);
    }
}
