// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class Project
{
    [Parameter]
    public string ProjectId { get; set; } = default!;

    [Parameter]
    public string AppId { get; set; }

    [Parameter]
    public Guid TeamId { get; set; }

    private bool _isLoading = true;

    public TeamDto Team { get; set; } = new();

    public ProjectDto Value { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isLoading)
        {
            Team = await ApiCaller.TeamService.GetTeamAsync(TeamId, ProjectId);
            Value = Team.CurrentProject;
            AppId = Team.CurrentProject.Apps?.FirstOrDefault()?.Identity!;
            _isLoading = false;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
