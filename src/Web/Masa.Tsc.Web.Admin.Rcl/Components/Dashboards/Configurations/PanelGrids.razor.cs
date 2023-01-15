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
    public bool IsEdit { get; set; }

    [CascadingParameter]
    public List<PanelGrids> PanelGridRange { get; set; }

    public bool IsEditTabItem { get; set; }

    public MGridstack<UpsertPanelDto>? Gridstack;

    protected override void OnInitialized()
    {
        PanelGridRange.Add(this);
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
        RemovePanelGrid(panel);

        if (panel.ParentPanel is null)
            Panels.Remove(panel);
        else
            panel.ParentPanel.ChildPanels.Remove(panel);
    }

    void RemovePanelGrid(UpsertPanelDto panel)
    {
        PanelGridRange.RemoveAll(item => item.ParentPanel == panel);
        if (panel.ChildPanels.Any())
        {
            //PanelGridRange.RemoveAll(item => item.Panels.Any(item2 => item2.ParentPanel?.Id == panel.Id));
            foreach (var item in panel.ChildPanels)
            {
                RemovePanelGrid(item);
            }
        }
    }

    async Task ReplacePanel(UpsertPanelDto panel)
    {
        var data = Panels.First(p => p.Id == panel.Id);
        panel.X = data.X;
        panel.Y = data.Y;
        panel.Width = data.Width;
        panel.Height = data.Height;
        Panels.Remove(data);
        Panels.Add(panel);
        if (panel.PanelType is PanelTypes.Chart)
        {
            await ConfigurationChartPanel(panel);
        }
    }

    async Task ConfigurationChartPanel(UpsertPanelDto panel)
    {
        await PanelGridRange.First(item => item.ParentPanel is null).Gridstack!.Reload();
        await Task.WhenAll(PanelGridRange.Select(item => item.SavePanelGridAsync()));
        NavigationManager.NavigateToConfigurationChart(panel.Id.ToString());
    }

    void OnResize(GridstackResizeEventArgs args)
    {
        var panel = Panels.FirstOrDefault(p => p.Id.ToString() == args.Id);
        if (panel is null) return;
        panel.W = args.Width;
        panel.H = args.Height;
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
