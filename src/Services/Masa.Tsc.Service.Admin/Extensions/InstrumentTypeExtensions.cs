// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Enums;

internal static class InstrumentTypeExtensions
{
    public static PanelDto ToModel(this InstrumentTypes type, Panel panel)
    {
        switch (type)
        {
            case InstrumentTypes.Text:
                return GetTextPanel(panel);
            default:
                return default!;
        }
    }

    private static PanelDto GetTextPanel(Panel panel)
    {
        return new TextPanelDto
        {
            Title = panel.Title,
            Id = panel.Id,
            ParentId = panel.ParentId,
            Height = panel.Height,
            Width = panel.Width,
            InstrumentId = panel.InstrumentId,
            Sort = panel.Sort,
            Type = panel.Type,
            Description = panel.Description
        };
    }

    private static PanelDto GetChartPanel(Panel panel)
    {
        return new ChartPanelDto
        {
            ChartType = panel.ChartType,
            Id = panel.Id,
            Description = panel.Description,
            Height = panel.Height,
            InstrumentId = panel.InstrumentId,
            ParentId = panel.ParentId,
            Sort = panel.Sort,
            Title = panel.Title,
            Type = panel.Type,
            Width = panel.Width,
            //Metrics =panel.Metrics
        };
    }

    private static PanelDto GetTabsPanel(Panel panel)
    {
        return new TabsPanelDto
        {
            Id = panel.Id,
            Description = panel.Description,
            Height = panel.Height,
            InstrumentId = panel.InstrumentId,
            ParentId = panel.ParentId,
            Sort = panel.Sort,
            Title = panel.Title,
            Type = panel.Type,
            Width = panel.Width,
            //Tabs = 
            //Metrics =panel.Metrics
        };
    }
}