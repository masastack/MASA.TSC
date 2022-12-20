// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    public string _projectId { get; set; }
    public Guid _teamId { get; set; }
    private string _appId = string.Empty;
    private TeamDto _team = default!;
    private ProjectCharts _projectCharts = new();

    private async Task LoadChartAsync(ProjectAppSearchModel query)
    {
        if (_projectCharts != null)
            await _projectCharts.OnLoadDataAsyc(query);
    }

    public async Task RefrshDataASync(Guid teamId, string projectId)
    {
        Visible = true;
        if (!string.IsNullOrEmpty(projectId) && teamId != Guid.Empty && _projectId != projectId && _teamId != teamId)
        {
            _team = await ApiCaller.TeamService.GetTeamAsync(teamId, projectId);
            _projectId = projectId;
            _teamId = teamId;
        }
        if (_team.CurrentProject != null && _team.CurrentProject.Apps != null && _team.CurrentProject.Apps.Any())
        {
            var first = _team.CurrentProject.Apps.First();
            if (first.Identity != _appId)
                _appId = first.Identity;
        }
        StateHasChanged();
    }
}
