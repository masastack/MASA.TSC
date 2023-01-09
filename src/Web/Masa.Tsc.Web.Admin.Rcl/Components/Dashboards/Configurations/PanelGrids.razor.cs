﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

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

    [CascadingParameter(Name = "DashboardId")]
    public string DashboardId { get; set; }

    [CascadingParameter(Name = "ServiceName")]
    public string? ServiceName { get; set; }

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
            await Task.WhenAll(PanelGridRange.Select(item => item.SavePanelGridAsync()));
            NavigationManager.NavigateToConfigurationChart(DashboardId, panel.Id.ToString(), ServiceName);
        }
    }

    void ConfigurationChartPanel(UpsertPanelDto panel)
    {
        NavigationManager.NavigateToConfigurationChart(DashboardId, panel.Id.ToString(), ServiceName);
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
