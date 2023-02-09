// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations.Models;

public class ConfigurationRecord
{
    public List<UpsertPanelDto> Panels { get; set; } = new();

    public string? AppName { get; set; }

    public string Search { get; set; }

    public string DashboardId { get; set; }

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;

    public string? Key => AppName + StartTime + EndTime + RandomStr;

    public bool IsEdit { get; set; }

    public bool ShowServiceCompontent { get; set; }

    string RandomStr { get; set; } = "";

    public void UpdateKey() => RandomStr = Guid.NewGuid().ToString();

    public void Clear()
    {
        Panels.Clear();
        //AppName = "";
        Search = "";
        DashboardId = "";
        IsEdit = false;
        ShowServiceCompontent = false;
        //StartTime = default;
        //EndTime = default;
    }
}
