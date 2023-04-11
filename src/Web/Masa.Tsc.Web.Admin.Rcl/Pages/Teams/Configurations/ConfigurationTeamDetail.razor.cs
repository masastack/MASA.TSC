// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams.Configurations;

public partial class ConfigurationTeamDetail
{
    ServiceAutoComplete _serviceAutoComplete;

    [Inject]
    public TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public string ServiceName { get; set; }

    [Parameter]
    public string ProjectId { get; set; }

    [Parameter]
    public string TeamId { get; set; }

    protected override void OnInitialized()
    {
        ConfigurationRecord.Clear();
        ConfigurationRecord.Service = ServiceName;
        ConfigurationRecord.ProjectId = ProjectId;
        ConfigurationRecord.TeamId = Guid.Parse(TeamId);
    }

    async Task SavePanelsAsync(List<UpsertPanelDto> panels)
    {
        await ApiCaller.InstrumentService.UpdateTeamInstraumentAsync(panels.ToArray());
    }

    async Task<List<UpsertPanelDto>> GetPanelsAsync()
    {
        return await ApiCaller.InstrumentService.GetTeamInstraumentDetailAsync();
    }

    void NavigateToTeamProjectDialog()
    {
        ConfigurationRecord.TeamProjectDialogVisible = true;
        ConfigurationRecord.NavigateToTeamProjectDialog();
    }
}
