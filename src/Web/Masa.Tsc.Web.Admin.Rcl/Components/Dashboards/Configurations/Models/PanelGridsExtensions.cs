// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Models;

public static class PanelGridsExtensions
{
    public static void AdaptiveUI(this List<UpsertPanelDto> panels, UpsertPanelDto panel)
    {
        if (panel.AutoPosition == false)
        {
            if (panels.Count == 0)
            {
                panels.Add(panel);
                return;
            }
            var ys = panels.Select(panel => panel.Y).Distinct().ToList();
            foreach(var itemPanel in panels.OrderByDescending(panel => panel.Y))
            {
                if (ys.Count > 1 && itemPanel.Y != 0)
                {
                    var onTopY = ys.Where(y => y < itemPanel.Y).Max();
                    var onTopPanels = panels.Where(panel => panel.Y == onTopY);
                    var rightPanel = onTopPanels.OrderByDescending(panel => panel.X).First();
                    if (rightPanel.X + rightPanel.Width + panel.Width > 12)
                    {
                        if ((itemPanel.X + panel.Width + itemPanel.Width) > 12)
                        {
                            itemPanel.X = 0;
                            itemPanel.Y += panel.Height;
                        }
                        else
                        {
                            itemPanel.X += panel.Width;
                        }
                    }
                    else itemPanel.X += panel.Width;
                }
                else if ((itemPanel.X + panel.Width + itemPanel.Width) > 12)
                {
                    itemPanel.X = 0;
                    itemPanel.Y += panel.Height;
                }
                else
                {
                    itemPanel.X += panel.Width;
                }
            }
        }
        panels.Insert(0, panel);
    }
}
