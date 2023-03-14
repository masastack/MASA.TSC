// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations.Models;

public class ConfigurationRecord
{
    public List<UpsertPanelDto> Panels { get; set; } = new();

    public string DashboardId { get; set; }

    public string? Service { get; set; }

    public string? Instance { get; set; }

    public string? Endpoint { get; set; }

    public string PanelId { get; set; }

    public ModelTypes ModelType { get; set; }

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;

    public string? Key => $"{Service}{Instance}{Endpoint}{StartTime}{EndTime}";

    public bool IsEdit { get; set; }

    public bool ServiceRelationReady(bool ready)
    {
        var allModelPass = ModelType is ModelTypes.All or default(ModelTypes);
        var serviceModelPass = ModelType is ModelTypes.Service && string.IsNullOrEmpty(Service) is false;
        var instanceModelPass = ModelType is ModelTypes.ServiceInstance && string.IsNullOrEmpty(Service) is false && string.IsNullOrEmpty(Instance) is false;
        var endPointPass = ModelType is ModelTypes.Endpoint && string.IsNullOrEmpty(Service) is false && string.IsNullOrEmpty(Instance) is false && string.IsNullOrEmpty(Endpoint) is false;
        return Panels.Any() && (ready || (allModelPass || serviceModelPass || instanceModelPass || endPointPass));
    }

    public void Clear()
    {
        ClearPanels();
        DashboardId = "";
        IsEdit = false;
    }

    public void ClearPanels()
    {
        Panels.ForEach(item => item.IsRemove = true);
        Panels.Clear();
    }
}
