// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Enums;

internal static class InstrumentTypeExtensions
{
    public static PanelDto ToModel(this PanelTypes type, Panel panel)
    {
        switch (type)
        {
            case PanelTypes.Text:
                return GetTextPanel(panel);
            case PanelTypes.Tabs:
                return GetTabsPanel(panel);
            case PanelTypes.TabItem:
                return GetTabItemPanel(panel);
            case PanelTypes.Chart:
                return GetChartPanel(panel);
            default:
                return GetDefault(panel);
        }
    }

    private static PanelDto GetDefault(Panel panel, PanelDto? result = null)
    {
        if (result == null)
            result = new PanelDto()
            {
                Type = panel.Type,
            };
        result.Id = panel.Id;
        result.Title = panel.Title;
        result.ParentId = panel.ParentId;
        result.Height = panel.Height;
        result.Width = panel.Width;
        result.InstrumentId = panel.InstrumentId;
        result.Sort = panel.Sort;
        result.Type = panel.Type;
        result.Description = panel.Description;
        return result;
    }

    private static PanelDto GetTextPanel(Panel panel)
    {
        return GetDefault(panel, new TextPanelDto());
    }

    private static PanelDto GetChartPanel(Panel panel)
    {
        var result = GetDefault(panel, new EChartPanelDto { ChartType = panel.ChartType });
        ((EChartPanelDto)result).Metrics = panel.Metrics.Select(item => new PanelMetricDto
        {
            Caculate = item.Caculate,
            Color = item.Color,
            Id = item.Id,
            Name = item.Name,
            Sort = item.Sort,
            Unit = item.Unit,
        }).ToList();
        return result;
    }

    private static PanelDto GetTabsPanel(Panel panel)
    {
        return GetDefault(panel, new TabsPanelDto());
    }

    private static PanelDto GetTabItemPanel(Panel panel)
    {
        return GetDefault(panel, new TabItemPanelDto());
    }
}