﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin;

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

    [CascadingParameter(Name = "ProjectId")]
    public string ProjectId { get; set; }

    [CascadingParameter(Name = "TeamId")]
    public Guid TeamId { get; set; }

    [CascadingParameter(Name = "ProjectCount")]
    public int ProjectCount { get; set; }

    [CascadingParameter(Name = "ServiceCount")]
    public int ServiceCount { get; set; }

    [CascadingParameter(Name = "ErrorCount")]
    public int ErrorCount { get; set; }

    [CascadingParameter]
    public ProjectOverviewDto Project { get; set; }

    [CascadingParameter(Name = "Start")]
    public DateTimeOffset Start { get; set; }

    [CascadingParameter(Name = "End")]
    public DateTimeOffset End { get; set; }

    [CascadingParameter]
    public QuickRangeKey? QuickRangeKey { get; set; }

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
            Team.ProjectTotal = ProjectCount;
            Team.AppTotal = ServiceCount;
            Team.Admins = new List<UserDto> { Team.CurrentProject.Creator, Team.CurrentProject.Creator, Team.CurrentProject.Creator };
            ConfigurationRecord.AppName = Apps.FirstOrDefault()?.Identity;
            ErrorCount = await GetErroCountAsync(ConfigurationRecord.AppName!);
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

    async Task OnAppChanged(string appid)
    {
        ConfigurationRecord.AppName = appid;
        //var app = Project?.Apps?.FirstOrDefault(app => app.Identity == appid);
        ErrorCount = await GetErroCountAsync(appid);
    }

    async Task<int> GetErroCountAsync(string appid)
    {
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
            Service = appid,
            Name = "Resource.service.name",
            Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                Name="SeverityText",
                Type= ConditionTypes.Equal,
                Value="Error"
                }
            }
        };
        return await ApiCaller.LogService.AggregateAsync<int>(query);
    }
}
