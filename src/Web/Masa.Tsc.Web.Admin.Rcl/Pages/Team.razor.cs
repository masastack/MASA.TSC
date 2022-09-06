// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = default!;
    private TeamSearch _teamSearch = default!;
    private int _error, _warn, _monitor, _normal;
    private bool _isLoad = true;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    private async Task LoadData()
    {
        long start = 0, end = 0;
        if (_teamSearch.Start.HasValue)
            start = _teamSearch.Start.Value.ToUnixTimestamp();
        if (_teamSearch.End.HasValue)
            end = _teamSearch.End.Value.ToUnixTimestamp();
        var data = await ApiCaller.ProjectService.OverviewAsync(new RequestTeamMonitorDto
        {
            EndTime = end,
            StartTime = start,
            Keyword = _teamSearch.Keyword,
            ProjectId = _teamSearch.ProjectId,
            UserId = CurrentUserId
        });

        if (data != null)
        {
            _error = data.Monitor.Error;
            _warn = data.Monitor.Warn;
            _monitor = data.Monitor.Total;
            _normal = data.Monitor.Normal;
            _projects = data.Projects;
        }
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        if (_isLoad)
        {
            await LoadData();
            StateHasChanged();
            _isLoad = false;
        }
        base.OnAfterRender(firstRender);
    }
}
