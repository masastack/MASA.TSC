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

    protected override void OnParametersSet()
    {
        if(PanelGridRange.Contains(this) is false)
        {
            PanelGridRange.Add(this);
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
        RemovePanelGrid(panel);

        if (panel.ParentPanel is null)
            Panels.Remove(panel);
        else
            panel.ParentPanel.ChildPanels.Remove(panel);

        if (Panels.Any() is false)
        {
            PanelGridRange.Remove(this);
        }
    }

    void RemovePanelGrid(UpsertPanelDto panel)
    {
        PanelGridRange.RemoveAll(item => item.ParentPanel == panel);
        if (panel.ChildPanels.Any())
        {
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
        if(data.Width != GlobalPanelConfig.Width || data.Height != GlobalPanelConfig.Height)
        {
            panel.Width = data.Width;
            panel.Height = data.Height;
        }
        panel.Id = Guid.NewGuid();
        Panels.Remove(data);
        await Task.Delay(10);
        Panels.Add(panel);
        if (panel.PanelType is PanelTypes.Chart)
        {
            await ConfigurationChartPanel(panel);
        }
    }

    async Task ConfigurationChartPanel(UpsertPanelDto panel)
    {
        await PanelGridRange.SaveUI();
        NavigationManager.NavigateToConfigurationChart(panel.Id.ToString());
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

    public void UpdateTimeZoneInfo(TimeZoneInfo timeZoneInfo)
    { 
        CurrentTimeZone= timeZoneInfo;
    }
}
