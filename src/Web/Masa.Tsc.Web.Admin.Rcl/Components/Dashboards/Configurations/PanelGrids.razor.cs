// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelGrids
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public List<UpsertPanelDto> Panels { get; set; }

    [CascadingParameter]
    public bool IsEdit { get; set; }

    [CascadingParameter]
    public List<PanelGrids> PanelGridRange { get; set; }

    public bool IsEditTabItem { get; set; }

    private MGridstack<UpsertPanelDto>? Gridstack;

    protected override void OnInitialized()
    {
        PanelGridRange.Add(this);
        IniPanels(Panels);
    }

    void IniPanels(List<UpsertPanelDto> panels, UpsertPanelDto? parentPanel = null)
    {
        foreach (var panel in panels)
        {
            panel.ParentPanel = parentPanel;
            if (panel.ChildPanels.Any())
            {
                IniPanels(panel.ChildPanels, panel);
            }
        }
    }

    void AddChildPanel(UpsertTabsPanelDto? panel)
    {
        panel?.CurrentTabItem?.AddPanel();
    }

    void AddTabItem(UpsertTabsPanelDto? panel)
    {
        panel?.AddTabItem();
    }

    void EditEditTabItems()
    {
        IsEditTabItem = !IsEditTabItem;
    }

    void RemovePanel(UpsertPanelDto panel)
    {
        if (panel is UpsertTabItemPanelDto tabItem)
        {
            tabItem.RemovePanel(panel);
        }
        else if (panel.ParentPanel is null)
            Panels.Remove(panel);
        else
            panel.ParentPanel.ChildPanels.Remove(panel);
    }

    void ReplacePanel(UpsertPanelDto panel)
    {       
        var data = Panels.First(p => p.Id == panel.Id);
        panel.X = data.X;
        panel.Y = data.Y;
        panel.Width = data.Width;
        panel.Height = data.Height;
        Panels.Remove(data);
        Panels.Add(panel);
    }

    void ConfigurationChartPanel(UpsertPanelDto panel)
    {
        NavigationManager.NavigateTo($"/dashboard/configuration/chart/{panel.Id}");
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
