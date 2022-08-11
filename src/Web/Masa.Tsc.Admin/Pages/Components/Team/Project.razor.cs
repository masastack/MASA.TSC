// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class Project
{
    private TeamDto _team;

    private ProjectDto _project;
    //[Parameter]
    //public ProjectDto Value { get; set; } = default!;

    [Parameter]
    public string AppId { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(AppId) && (_team == null || _team.CurrentAppId != AppId))
        {
            _team = await ApiCaller.TeamService.GetTeamAsync(Guid.Empty, AppId);
            _project = _team.CurrentProject;
        }
        await base.OnParametersSetAsync();
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        _team = await ApiCaller.TeamService.GetTeamAsync(AppId);
    //        _project = _team.CurrentProject;
    //    }
    //    await base.OnAfterRenderAsync(firstRender);
    //}
}
