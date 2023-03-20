// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams.Configurations;

public partial class ConfigurationTeamDetail
{
    [Inject]
    public TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public string ServiceName { get; set; }

    protected override void OnInitialized()
    {
        if (ConfigurationRecord.NavigationManager.Uri.Contains("record") is false)
        {
            ConfigurationRecord.Clear();
        }

        ConfigurationRecord.Service = ServiceName;
    }

    async Task SavePanelsAsync(List<UpsertPanelDto> panels)
    {
        await ApiCaller.InstrumentService.UpdateTeamInstraumentAsync(panels.ToArray());
    }

    async Task<List<UpsertPanelDto>> GetPanelsAsync()
    {
        return await ApiCaller.InstrumentService.GetTeamInstraumentDetailAsync();
    }
}
