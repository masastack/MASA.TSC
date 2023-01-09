// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = new();
    private List<ProjectOverviewDto> _viewProjects = new();
    private TeamSearchModel? _teamSearchModel = null;
    private AppMonitorDto _appMonitorDto;
    private bool _isLoad = true;
    private StringNumber _projectStatus ="";

    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            StateHasChanged();
        }
    }

    private async Task LoadData()
    {
        _isLoad = true;
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
            _viewProjects = GetViewData();
        }
        _isLoad = false;
    }

    private async Task OnSearch(TeamSearchModel query)
    {
        _teamSearchModel = query;
        await LoadData();
    }

    private void OnTabsChange(TeamSearchModel query)
    {
        _teamSearchModel = query;
        _viewProjects = GetViewData();
    }

    void ProjectStatusChanged(StringNumber projectStatus)
    {
        _projectStatus = projectStatus;
        _viewProjects = GetViewData();
    }

    List<ProjectOverviewDto> GetViewData()
    {
        IEnumerable<ProjectOverviewDto>? result = _projects;
        if (_projectStatus != "MONITORING" && _projectStatus != "")
        {
            result = result.Where(item => item.Status.ToString().Equals(_projectStatus.ToString(), StringComparison.OrdinalIgnoreCase));
        }
        if(_teamSearchModel?.ProjectType!="all" && string.IsNullOrEmpty(_teamSearchModel?.ProjectType) is false)
        {
            result = result.Where(item => item.LabelName.Equals(_teamSearchModel.ProjectType, StringComparison.OrdinalIgnoreCase));
        }
        return result.ToList();
    }
}
