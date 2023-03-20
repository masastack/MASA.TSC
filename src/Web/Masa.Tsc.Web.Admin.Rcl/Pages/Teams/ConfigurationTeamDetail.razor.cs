// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams;

public partial class ConfigurationTeamDetail
{
    [Inject]
    public TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public string ServiceName { get; set; }

    protected override void OnInitialized()
    {
        if (ConfigurationRecord.NavigationManager.Uri.Contains("record")) return;

        ConfigurationRecord.Clear();
        ConfigurationRecord.Service = ServiceName;

    }

    async Task SavePanelsAsync(List<UpsertPanelDto> panels)
    {
        throw new NotImplementedException();
    }

    async Task<List<UpsertPanelDto>> GetPanelsAsync()
    {
        throw new NotImplementedException();
    }
}
