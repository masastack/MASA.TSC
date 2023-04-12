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
            for (int i = 0; i < panels.Count; i++)
            {
                var itemPanel = panels[i];
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
        }
        panels.Add(panel);
    }
}
