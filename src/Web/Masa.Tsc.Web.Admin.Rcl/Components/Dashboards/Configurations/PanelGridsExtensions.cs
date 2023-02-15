﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public static class PanelGridsExtensions
{
    public static async Task SaveUI(this IEnumerable<PanelGrids> panelGrids)
    {
        if (panelGrids.Any() is false) return;
        await panelGrids.First(item => item.ParentPanel is null).Gridstack!.Reload();
        await Task.WhenAll(panelGrids.Select(item => item.SavePanelGridAsync()));
    }

    public static void AdaptiveUI(this List<UpsertPanelDto> panels, UpsertPanelDto panel)
    {
        if (panels.Count == 0)
        {
            panels.Add(panel);
            return;
        }
        //panels.ForEach(panel => panel.Y = panel.Y + GlobalPanelConfig.Height);
        //var minYPanels = panels.Where(panel => panel.Y == GlobalPanelConfig.Height);
        //var sumX = minYPanels.Sum(panel => panel.Width);
        var maxY = panels.Max(panel => panel.Y);
        var maxYpanels = panels.Where(panel => panel.Y == maxY);
        var sumX = maxYpanels.Sum(panel => panel.Width);
        if (sumX + GlobalPanelConfig.Width <= 12)
        {
            panel.X = sumX;
        }
        panel.Y = 10000;
        panels.Add(panel);
    }
}