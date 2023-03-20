// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelGrids
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public List<UpsertPanelDto> Panels { get; set; }

    [Parameter]
    public UpsertPanelDto? ParentPanel { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsEdit => ConfigurationRecord.IsEdit;

    [CascadingParameter]
    public List<PanelGrids> PanelGridRange { get; set; }

    public MGridstack<UpsertPanelDto>? Gridstack;

    protected override void OnParametersSet()
    {
        if (PanelGridRange.Contains(this) is false)
        {
            PanelGridRange.Add(this);
        }
    }

    async Task AddChildPanel(UpsertTabsPanelDto? panel)
    {
        await PanelGridRange.SaveUI();
        panel?.CurrentTabItem?.AddPanel();
    }

    void RemovePanel(UpsertPanelDto panel)
    {
        panel.IsRemove = true;
        Panels.Remove(panel);
    }

    async Task ReplacePanel(UpsertPanelDto panel)
    {
        var data = Panels.First(p => p.Id == panel.Id);
        await SavePanelGridAsync();
        panel.X = data.X;
        panel.Y = data.Y;
        if (data.Width != GlobalPanelConfig.Width || data.Height != GlobalPanelConfig.Height)
        {
            panel.Width = data.Width;
            panel.Height = data.Height;
        }
        else
        {
            switch (panel.PanelType)
            {
                case PanelTypes.Tabs:
                    panel.Width = 12;
                    panel.Height = 6;
                    break;
                case PanelTypes.Chart:
                    panel.Width = 12;
                    panel.Height = 5;
                    break;
                case PanelTypes.Log:
                    panel.Width = 12;
                    panel.Height = 10;
                    break;
                case PanelTypes.Trace:
                    panel.Width = 12;
                    panel.Height = 9;
                    break;
                case PanelTypes.Topology:
                    panel.Width = 12;
                    panel.Height = 6;
                    break;
                default: break;
            }
        }
        panel.Id = Guid.NewGuid();
        Panels.Remove(data);
        StateHasChanged();
        Panels.Add(panel);
        if (panel.PanelType is PanelTypes.Chart)
        {
            await ConfigurationChartPanel(panel);
        }
    }

    async Task ConfigurationChartPanel(UpsertPanelDto panel)
    {
        await PanelGridRange.SaveUI();
        ConfigurationRecord.PanelId = panel.Id.ToString();
        ConfigurationRecord.NavigateToChartConfiguration();
    }

    public async Task SavePanelGridAsync()
    {
        var grids = await Gridstack!.OnSave();
        foreach (var grid in grids)
        {
            var panel = Panels.First(p => p.Id == Guid.Parse(grid.Id));
            panel.Width = grid.W;
            panel.Height = grid.H;
            panel.X = grid.X ?? 0;
            panel.Y = grid.Y ?? 0;
        }
    }
}
