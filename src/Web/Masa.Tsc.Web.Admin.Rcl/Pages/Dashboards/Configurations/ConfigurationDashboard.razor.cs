// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class ConfigurationDashboard
{
    [Inject]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    [Parameter]
    public string? ServiceName { get; set; }

    [Parameter]
    public string? InstanceName { get; set; }

    [Parameter]
    public string? EndpointName { get; set; }

    protected override void OnInitialized()
    {
        if (ConfigurationRecord.NavigationManager.Uri.Contains("record")) return;
        ConfigurationRecord.Clear();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (ConfigurationRecord.HasNavigateTo)//想想办法解决这个问题，去掉这段代码
        {
            ConfigurationRecord.HasNavigateTo = false;
            return;
        }
        ConfigurationRecord.Service = ServiceName;
        ConfigurationRecord.Instance = InstanceName;
        ConfigurationRecord.Endpoint = EndpointName;
        if (string.IsNullOrEmpty(DashboardId) is false && ConfigurationRecord.DashboardId != DashboardId)
        {
            ConfigurationRecord.DashboardId = DashboardId;
            ConfigurationRecord.ModelType = default;
            await ConfigurationRecord.BindPanelsAsync(ApiCaller);
        }
    }
}
