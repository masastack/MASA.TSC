// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations.Models;

public class ConfigurationRecord
{
    public List<UpsertPanelDto> Panels { get; set; } = new();

    public string DashboardId { get; set; }

    public string? Service { get; set; }

    public string? Relation { get; set; }

    public string Search { get; set; }

    public ModelTypes ModelType { get; set; }

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;

    public string? Key => $"{Service}{StartTime}{EndTime}{RandomStr}";

    public bool IsEdit { get; set; }

    public bool NotRelationData { get; set; }

    public bool RenderReady => Panels.Any() && (
        ModelType is ModelTypes.All || 
        (ModelType is ModelTypes.Service && string.IsNullOrEmpty(Service) is false) || 
        (string.IsNullOrEmpty(Service) is false && (string.IsNullOrEmpty(Relation) is false || NotRelationData))
    );

    string RandomStr { get; set; } = "";

    public void UpdateKey() => RandomStr = Guid.NewGuid().ToString();

    public void Clear()
    {
        ClearPanels();
        Search = "";
        DashboardId = "";
        IsEdit = false;
    }

    public void ClearPanels()
    {
        Panels.ForEach(item => item.IsRemove = true);
        Panels.Clear();
    }
}
