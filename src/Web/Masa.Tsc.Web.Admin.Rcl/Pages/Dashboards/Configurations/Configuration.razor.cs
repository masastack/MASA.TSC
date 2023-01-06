﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration
{
    [Inject]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string? DashboardId { get; set; }

    [Parameter]
    public string ServiceName { get; set; }

    List<PanelGrids> PanelGrids { get; set; } = new();

    bool IsEdit { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        if (DashboardId is null)
        {
            return;
        }
        ConfigurationRecord.Clear();
        ConfigurationRecord.DashboardId = DashboardId;
        ConfigurationRecord.AppName = ServiceName;
        await GetPanelsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if(DashboardId is not null && ConfigurationRecord.DashboardId != DashboardId)
        {
            ConfigurationRecord.Clear();
            ConfigurationRecord.DashboardId = DashboardId;
            ConfigurationRecord.AppName = ServiceName;
            PanelGrids.Clear();
            await GetPanelsAsync();
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (string.IsNullOrEmpty(ConfigurationRecord.DashboardId)) NavigationManager.NavigateTo($"/dashboard");
    }

    async Task GetPanelsAsync()
    {
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(ConfigurationRecord.DashboardId));
        if (detail?.Panels != null && detail.Panels.Any())
            ConfigurationRecord.Panels.AddRange(detail.Panels);

        Convert(ConfigurationRecord.Panels);
    }

    void Convert(List<UpsertPanelDto> panels, UpsertPanelDto? parentPanel = null)
    {
        panels.ForEach(panel => panel.ParentPanel = parentPanel);
        var chartPanels = panels.Where(panel => panel.PanelType == PanelTypes.Chart).ToList();
        var tabsPanels = panels.Where(panel => panel.PanelType == PanelTypes.Tabs).ToList();
        var tabItemPanels = panels.Where(panel => panel.PanelType == PanelTypes.TabItem).ToList();
        panels.RemoveAll(panel => panel.PanelType == PanelTypes.Chart || panel.PanelType == PanelTypes.Tabs || panel.PanelType == PanelTypes.TabItem);
        panels.AddRange(chartPanels.Select(panel => new UpsertChartPanelDto(default).Clone(panel)));
        panels.AddRange(tabsPanels.Select(panel => new UpsertTabsPanelDto(default).Clone(panel)));
        panels.AddRange(tabItemPanels.Select(panel => new UpsertTabItemPanelDto(parentPanel as UpsertTabsPanelDto).Clone(panel)));
        foreach (var panel in panels)
        {
            if (panel.ChildPanels.Any() is false) continue;
            else Convert(panel.ChildPanels, panel);
        }
    }

    void AddPanel()
    {
        ConfigurationRecord.Panels.Insert(0, new());
    }

    void OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await base.InvokeAsync(base.StateHasChanged);
    }

    async Task SaveAsync()
    {
        if(ConfigurationRecord.Panels.Any() is false)
        {
            PanelGrids.Clear();
        }
        else
        {
            await PanelGrids.First(item => item.ParentPanel is null).Gridstack!.Reload();// dont not remove
            await Task.WhenAll(PanelGrids.Select(item => item.SavePanelGridAsync()));
        }
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(ConfigurationRecord.DashboardId), ConfigurationRecord.Panels.ToArray());
        OpenSuccessMessage(T("Save success"));
    }
}
