// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
    string _projectId { get; set; }

    Guid _teamId { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public string ProjectId { get; set; }

    [Parameter]
    public Guid TeamId { get; set; }

    [Parameter]
    public int MonitorProjectCount { get; set; }

    [Parameter]
    public int MonitorServiceCount { get; set; }

    ConfigurationRecord ConfigurationRecord { get; set; } = new();

    ServiceAutoComplete AppAutoComplete { get; set; }

    List<AppDetailModel> Apps { get; set; } = new();

    TeamDto Team { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && !string.IsNullOrEmpty(ProjectId) && TeamId != Guid.Empty && (ProjectId != _projectId || TeamId != _teamId))
        {
            _projectId = ProjectId;
            _teamId = TeamId;
            Team = await ApiCaller.TeamService.GetTeamAsync(TeamId, ProjectId);
            Apps = Team.CurrentProject.Apps.Select(app => new AppDetailModel
            {
                Name = app.Name,
                Identity = app.Identity,
                Type = app.AppType,
                ServiceType = app.ServiceType
            }).ToList();
            Team.ProjectTotal = MonitorProjectCount;
            Team.AppTotal = MonitorServiceCount;
            ConfigurationRecord.Service = Apps.FirstOrDefault()?.Identity;
        }
    }

    void OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await base.InvokeAsync(base.StateHasChanged);
    }

    async Task NavigateToDashboardConfiguration()
    {
        var data = await base.ApiCaller.InstrumentService.GetLinkAsync(MetricValueTypes.Service);
        if (data?.InstrumentId is not null)
        {
            NavigationManager.NavigateToDashboardConfiguration(data.InstrumentId.ToString()!, ConfigurationRecord.Service);
        }
    }
}
