// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration
{
    [Inject]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public string? DashboardId { get; set; }

    bool IsEdit { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        if (DashboardId is null)
        {
            return;
        }
        ConfigurationRecord.Clear();
        ConfigurationRecord.DashboardId = DashboardId;
        await GetPanelsAsync();
    }

    async Task GetPanelsAsync()
    {
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(ConfigurationRecord.DashboardId));
        if (detail.Panels != null && detail.Panels.Any())
            ConfigurationRecord.Panels.AddRange(detail.Panels);

        Convert(ConfigurationRecord.Panels);
    }

    void Convert(List<UpsertPanelDto> panels)
    {
        var chartPanels = panels.Where(panel => panel.PanelType == PanelTypes.Chart).ToList();
        var tabsPanels = panels.Where(panel => panel.PanelType == PanelTypes.Tabs).ToList();
        var tabItemPanels = panels.Where(panel => panel.PanelType == PanelTypes.TabItem).ToList();
        panels.RemoveAll(panel => panel.PanelType == PanelTypes.Chart || panel.PanelType == PanelTypes.Tabs || panel.PanelType == PanelTypes.TabItem);
        panels.AddRange(chartPanels.Select(panel => new UpsertChartPanelDto(default).Clone(panel)));
        panels.AddRange(tabsPanels.Select(panel => new UpsertTabsPanelDto(default).Clone(panel)));
        panels.AddRange(tabItemPanels.Select(panel => new UpsertTabItemPanelDto(default).Clone(panel)));
        foreach (var panel in panels)
        {
            if (panel.ChildPanels.Any() is false) continue;
            else Convert(panel.ChildPanels);
        }
    }

    void AddPanel()
    {
        ConfigurationRecord.Panels.Insert(0, new());
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task SaveAsync()
    {
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(ConfigurationRecord.DashboardId), ConfigurationRecord.Panels.ToArray());
        OpenSuccessMessage(T("Save success"));
    }
}
