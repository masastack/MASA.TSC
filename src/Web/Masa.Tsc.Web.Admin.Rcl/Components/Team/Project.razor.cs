// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class Project
{
    [Parameter]
    public string ProjectId { get; set; }

    [Parameter]
    public Guid TeamId { get; set; }

    private ProjectCharts _projectCharts;

    private string _appId { get; set; }    

    private bool _isLoading = true;

    private TeamDto _team { get; set; }

    private ProjectDto _project { get; set; }

    protected override void OnParametersSet()
    {
        _isLoading = true;
        base.OnParametersSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isLoading)
        {
            _team = await ApiCaller.TeamService.GetTeamAsync(TeamId, ProjectId);
            _project = _team.CurrentProject;
            _appId = _team.CurrentProject.Apps?.FirstOrDefault()?.Identity!;
            _isLoading = false;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
