// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;

public static class ConfigurationRecordExtensions
{
    public static async Task BindPanelsAsync(this DashboardConfigurationRecord configurationRecord, TscCaller apiCaller)
    {
        configurationRecord.ClearPanels();
        var detail = await apiCaller.InstrumentService.GetDetailAsync(Guid.Parse(configurationRecord.DashboardId));
        if (detail is not null)
        {
            configurationRecord.ModelType = Enum.Parse<ModelTypes>(detail.Model);
            if (detail.Panels?.Any() is true)
            {
                configurationRecord.Panels.AddRange(detail.Panels);
            }
        }

        if (configurationRecord.Panels.Any() is false) configurationRecord.IsEdit = true;
        Convert(configurationRecord.Panels);

        void Convert(List<UpsertPanelDto> panels, UpsertPanelDto? parentPanel = null)
        {
            panels.ForEach(panel => panel.ParentPanel = parentPanel);
            var chartPanels = panels.Where(panel => panel.PanelType == PanelTypes.Chart).ToList();
            var tabsPanels = panels.Where(panel => panel.PanelType == PanelTypes.Tabs).ToList();
            var tabItemPanels = panels.Where(panel => panel.PanelType == PanelTypes.TabItem).ToList();
            panels.RemoveAll(panel => panel.PanelType == PanelTypes.Chart || panel.PanelType == PanelTypes.Tabs || panel.PanelType == PanelTypes.TabItem);
            panels.AddRange(chartPanels.Select(panel => new UpsertChartPanelDto(Guid.Empty).Clone(panel)));
            panels.AddRange(tabsPanels.Select(panel => new UpsertTabsPanelDto(Guid.Empty).Clone(panel)));
            panels.AddRange(tabItemPanels.Select(panel => new UpsertTabItemPanelDto(parentPanel as UpsertTabsPanelDto).Clone(panel)));
            foreach (var panel in panels)
            {
                if (panel.ChildPanels.Any() is false) continue;
                else Convert(panel.ChildPanels, panel);
            }
        }
    }
}
