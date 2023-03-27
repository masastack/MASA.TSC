// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Models;

public abstract class ConfigurationRecord
{
    string? _randomStr;
    ModelTypes _modelType;
    bool _isEdit;

    public List<UpsertPanelDto> Panels { get; set; } = new();

    public string? Service { get; set; }

    public string? ServiceName { get; set; }

    public string? Instance { get; set; }

    public string? Endpoint { get; set; }

    public string? ConvertEndpoint => Endpoint == "All" ? null : Endpoint;

    public string? PanelId { get; set; }

    public ModelTypes ModelType
    {
        get => _modelType;
        set
        {
            if (value is ModelTypes.All)
            {
                Service = null;
                Instance = null;
                Endpoint = null;
            }
            _modelType = value;
        }
    }

    public string? Layer { get; set; }

    public QuickRangeKey? DefaultQuickRangeKey = QuickRangeKey.Last15Minutes;

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;

    public string? Key => $"{Service}{Instance}{Endpoint}{StartTime}{EndTime}{_randomStr}";

    public bool IsEdit
    {
        get => _isEdit;
        set
        {
            _isEdit = value;
            JSRuntime.InvokeVoidAsync("eval", $"window.isEdit = {value}");
        }
    }

    public Func<TopListOption, Task>? TopListOnclick { get; set; }

    public NavigationManager NavigationManager { get; }

    public IJSRuntime JSRuntime { get; }

    public ConfigurationRecord(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        NavigationManager = navigationManager;
        JSRuntime = jsRuntime;
    }

    public abstract void NavigateToConfiguration();

    public abstract void NavigateToConfigurationRecord();

    public abstract void NavigateToChartConfiguration();

    public bool ServiceRelationReady(bool ready)
    {
        var allModelPass = ModelType is ModelTypes.All;
        var serviceModelPass = ModelType is ModelTypes.Service && string.IsNullOrEmpty(Service) is false;
        var instanceModelPass = ModelType is ModelTypes.ServiceInstance && string.IsNullOrEmpty(Service) is false && string.IsNullOrEmpty(Instance) is false;
        var endPointPass = ModelType is ModelTypes.Endpoint && string.IsNullOrEmpty(Service) is false && string.IsNullOrEmpty(Instance) is false && string.IsNullOrEmpty(Endpoint) is false;
        return Panels.Any() && (ready || allModelPass || serviceModelPass || instanceModelPass || endPointPass);
    }

    public void ReloadUI()
    {
        _randomStr = Guid.NewGuid().ToString();
    }

    public virtual void Clear()
    {
        ClearPanels();
        ModelType = default;
        Layer = default;
        Service = default;
        Instance = default;
        Endpoint = default;
        PanelId = default;
        IsEdit = false;
        DefaultQuickRangeKey = QuickRangeKey.Last15Minutes;
        StartTime = DateTimeOffset.UtcNow.AddMinutes(-15);
        EndTime = DateTimeOffset.UtcNow;
    }

    public void ClearPanels()
    {
        Panels.ForEach(item => item.IsRemove = true);
        Panels.Clear();
    }
}
