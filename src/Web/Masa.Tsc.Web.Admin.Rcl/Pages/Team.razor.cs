// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = new();
    private TeamSearchModel? _teamSearchModel = null;
    private AppMonitorDto _appMonitorDto;
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
        DateTime start = DateTime.MinValue, end = DateTime.MinValue;
        if (_teamSearchModel != null)
        {
            if (_teamSearchModel.Start.HasValue)
                start = _teamSearchModel.Start.Value;
            if (_teamSearchModel.End.HasValue)
                end = _teamSearchModel.End.Value;
        }
        var data = await ApiCaller.ProjectService.OverviewAsync(new RequestTeamMonitorDto
        {
            EndTime = end,
            StartTime = start,
            Keyword = _teamSearchModel?.Keyword ?? default!,
            ProjectId = _teamSearchModel?.AppId ?? default!,
            UserId = CurrentUserId
        });

        _appMonitorDto = data.Monitor;
        if (data != null)
        {
            _projects = data.Projects;
        }
    }

    private async Task OnSearch(TeamSearchModel query)
    {
        _teamSearchModel = query;
        await LoadData();
        StateHasChanged();
    }
}
