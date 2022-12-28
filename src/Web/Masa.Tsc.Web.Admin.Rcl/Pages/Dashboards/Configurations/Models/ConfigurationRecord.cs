// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations.Models;

public class ConfigurationRecord
{
    public List<UpsertPanelDto> Panels { get; set; } = new();

    public int AppId { get; set; }

    public string Search { get; set; }

    public string DashboardId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public void Clear()
    {
        Panels.Clear();
        AppId = default;
        Search = "";
        DashboardId = "";
        StartTime = default;
        EndTime = default;
    }
}
