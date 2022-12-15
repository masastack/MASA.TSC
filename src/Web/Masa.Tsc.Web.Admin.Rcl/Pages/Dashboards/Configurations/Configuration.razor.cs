// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration
{
    [Parameter]
    public string DashboardId { get; set; }

    int AppId { get; set; }

    string Search { get; set; }

    bool IsEdit { get; set; } = true;

    DateTimeOffset StartTime { get; set; } = DateTimeOffset.Now.AddDays(-1);

    DateTimeOffset EndTime { get; set; } = DateTimeOffset.Now;

    List<UpsertPanelDto> Panels { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (Panels.Any() is false)
        {
            Panels.Add(new());
        }

        await Task.CompletedTask;
    }

    List<PanelTypes> GetPanelTypes(PanelTypes type = default)
    {
        if (type == default)
        {
            return new List<PanelTypes>
            {
                PanelTypes.Tabs,
                PanelTypes.Text,
                PanelTypes.Chart,
                PanelTypes.Topology,
                PanelTypes.Log,
                PanelTypes.Trace,
                PanelTypes.Table
            };
        }
        return new List<PanelTypes>();
    }

    void AddPanel()
    {
        Panels.Insert(0, new());
    }
}
