// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class ConfigurationDashboard
{
    [Inject]
    public DashboardConfigurationRecord ConfigurationRecord { get; set; }

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
        ConfigurationRecord.Service = ServiceName;
        ConfigurationRecord.Instance = InstanceName;
        ConfigurationRecord.Endpoint = EndpointName;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(DashboardId) is false && ConfigurationRecord.DashboardId != DashboardId)
        {
            ConfigurationRecord.DashboardId = DashboardId;
            ConfigurationRecord.ModelType = default;
            await ConfigurationRecord.BindPanelsAsync(ApiCaller);
        }
    }

    async Task SavePanelsAsync(List<UpsertPanelDto> panels)
    {
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(ConfigurationRecord.DashboardId), panels.ToArray());
    }

    async Task<List<UpsertPanelDto>> GetPanelsAsync()
    {
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(ConfigurationRecord.DashboardId));
        if (detail is not null)
        {
            ConfigurationRecord.ModelType = Enum.Parse<ModelTypes>(detail.Model);
        }
        return detail?.Panels ?? new List<UpsertPanelDto>();
    }
}
