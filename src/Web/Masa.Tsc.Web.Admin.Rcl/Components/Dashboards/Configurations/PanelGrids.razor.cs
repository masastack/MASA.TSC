// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class PanelGrids
{
    [Parameter]
    public UpsertPanelDto? ParentPanel { get; set; }

    [Parameter]
    public List<UpsertPanelDto> Panels { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsEdit => ConfigurationRecord.IsEdit;

    void AddChildPanel(UpsertTabsPanelDto? panel)
    {
        panel?.CurrentTabItem?.AddPanel();
    }

    void RemovePanel(UpsertPanelDto panel)
    {
        Panels.Remove(panel);
    }

    void ReplacePanel(UpsertPanelDto panel)
    {
        var index = Panels.FindIndex(p => p.Id == panel.Id);
        var data = Panels[index];
        Panels.Remove(data);
        panel.X = data.X;
        panel.Y = data.Y;
        panel.ParentPanel = data.ParentPanel;
        panel.Width = data.Width;
        panel.Height = data.Height;
        if ((data.Width != GlobalPanelConfig.Width || data.Height != GlobalPanelConfig.Height))
        {
            panel.Width = data.Width;
            panel.Height = data.Height;
        }
        else
        {
            switch (panel.PanelType)
            {
                case PanelTypes.Tabs:
                    panel.Width = 12;
                    panel.Height = 6;
                    break;
                case PanelTypes.Chart:
                    //panel.Width = 12;
                    //panel.Height = 4;
                    break;
                case PanelTypes.Log:
                    panel.Width = 12;
                    panel.Height = 10;
                    break;
                case PanelTypes.Trace:
                    panel.Width = 12;
                    panel.Height = 6;
                    break;
                case PanelTypes.Topology:
                    panel.Width = 12;
                    panel.Height = 6;
                    break;
                default: break;
            }
        }

        panel.Id = Guid.NewGuid();
        Panels.Insert(index, panel);
        if (panel.PanelType is PanelTypes.Chart)
        {
            panel.PanelType = PanelTypes.Select;
            ConfigurationChartPanel(panel);
        }
    }

    void ConfigurationChartPanel(UpsertPanelDto panel)
    {
        ConfigurationRecord.PanelId = panel.Id.ToString();
        ConfigurationRecord.NavigateToChartConfiguration();
    }
}
