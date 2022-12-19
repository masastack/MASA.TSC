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
        UpsertPanelDto panel = new();
        switch (type)
        {
            case PanelTypes.Tabs:
                panel = new UpsertPanelDto()
                {
                    Id = PanelId,
                    PanelType = type,
                };
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
            case PanelTypes.Chart:
                panel = new UpsertChartPanelDto(PanelId);
                NavigationManager.NavigateTo($"/datav/dashboard/example3/{panel.Id}");
                break;
            default:
                break;
        }        

        await OnSelect.InvokeAsync(panel);
    }
}
