// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams.Configurations;

public partial class ConfigurationTeamDetail
{
    ServiceAutoComplete _serviceAutoComplete;
    string? _serviceName;

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
        ConfigurationRecord.ModelType = ModelTypes.All;
        ConfigurationRecord.Service = ServiceName;
        ConfigurationRecord.ProjectId = ProjectId;
        ConfigurationRecord.TeamId = Guid.Parse(TeamId);
    }

    async Task SavePanelsAsync(List<UpsertPanelDto> panels)
    {
        await ApiCaller.InstrumentService.UpdateTeamInstrumentAsync(panels.ToArray());
    }

    async Task<List<UpsertPanelDto>> GetPanelsAsync()
    {
        return await ApiCaller.InstrumentService.GetTeamInstrumentDetailAsync();
    }

    void ServicesDataReady()
    {
        _serviceName = _serviceAutoComplete?.CurrentApp()?.Name ?? ConfigurationRecord.Service;
    }

    void NavigateToTeamProjectDialog()
    {
        ConfigurationRecord.TeamProjectDialogVisible = true;
        ConfigurationRecord.NavigateToTeamProjectDialog();
    }
}
