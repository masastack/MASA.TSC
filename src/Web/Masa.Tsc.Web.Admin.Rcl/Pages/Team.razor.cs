// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = default!;
    private TeamSearchModel? _teamSearchModel = null;
    private int _error, _warn, _monitor, _normal;
    private bool _isLoad = true;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
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

    private async Task LoadData()
    {
        long start = 0, end = 0;
        if (_teamSearchModel != null)
        {
            if (_teamSearchModel.Start.HasValue)
                start = _teamSearchModel.Start.Value.ToUnixTimestamp();
            if (_teamSearchModel.End.HasValue)
                end = _teamSearchModel.End.Value.ToUnixTimestamp();
        }
        var data = await ApiCaller.ProjectService.OverviewAsync(new RequestTeamMonitorDto
        {
            EndTime = end,
            StartTime = start,
            Keyword = _teamSearchModel?.Keyword ?? default!,
            ProjectId = _teamSearchModel?.AppId ?? default!,
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

    private async Task<IEnumerable<ProjectDto>> LoadProjectAsync()
    {
        if (_projects == null)
        {
            await LoadData();
        }

        if (_projects == null || !_projects.Any())
            return default!;
        return _projects.Select(x => new ProjectDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Identity = x.Identity,
            LabelName = x.LabelName,
            TeamId = x.TeamId,
            Apps = x.Apps,
        });
    }

    private async Task OnSearch(TeamSearchModel query)
    {
        _teamSearchModel = query;
        await LoadData();        
        StateHasChanged();
    }
}
