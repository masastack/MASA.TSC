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
    private StringNumber _projectStatus = "0";

    private bool _visible;
    private string _selectedProjectId;
    private Guid _selectedTeamId;
    private int _teamProjectCount = 0;
    private int _teamServiceCount = 0;

    private void HandleOnItemClick(ProjectOverviewDto item)
    {
        _selectedProjectId = item.Identity;
        _selectedTeamId = item.TeamId;
        _teamProjectCount = _projects.Count(p => p.TeamId == _selectedTeamId);
        _teamServiceCount = _projects.Where(p => p.TeamId == _selectedTeamId).Sum(p => p.Apps.Count);
        _visible = true;
    }

    private string CellBackBackgroundStyle(ProjectOverviewDto overviewDto)
    {
        return overviewDto.Status switch
        {
            MonitorStatuses.Normal => "background: #E6FAF5;",
            MonitorStatuses.Warn => "background: #FFF7E8;",
            MonitorStatuses.Error => "background: #FFECE8;",
            _ => ""
        };
    }

    private string CellBorderColor(ProjectOverviewDto overviewDto)
    {
        return overviewDto.Status switch
        {
            MonitorStatuses.Normal => "#CDF5EB",
            MonitorStatuses.Warn => "#FFE4BA",
            MonitorStatuses.Error => "#FDCDC5",
            _ => ""
        };
    }

    private string ChipLabelColor(AppDto appDto)
    {
        return appDto.Status switch
        {
            MonitorStatuses.Warn => "#FF7D00",
            MonitorStatuses.Error => "#FF5252",
            _ => "#66BB6A"
        };
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

        _appMonitorDto = data?.Monitor ?? new();
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

    private void OnSearchChange(TeamSearchModel query)
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
        if (_projectStatus != null && _projectStatus.IsT0 && Enum.TryParse<MonitorStatuses>(_projectStatus.AsT0, out var type))
        {
            switch (type)
            {
                case MonitorStatuses.Error:
                    result = result.Where(item => item.HasError);
                    break;
                case MonitorStatuses.Warn:
                    result = result.Where(item => item.HasWarning);
                    break;
                case MonitorStatuses.Normal:
                    result = result.Where(item => !item.HasWarning && !item.HasError);
                    break;
            }
        }

        if (_teamSearchModel?.ProjectType != "all" && !string.IsNullOrEmpty(_teamSearchModel?.ProjectType))
        {
            result = result.Where(item => item.LabelCode.Equals(_teamSearchModel.ProjectType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(_teamSearchModel?.Keyword))
        {
            result = result.Where(item => item.Name.Contains(_teamSearchModel.Keyword, StringComparison.OrdinalIgnoreCase) || item.Apps.Any(app => app.Name.Contains(_teamSearchModel.Keyword, StringComparison.OrdinalIgnoreCase)));
        }
        return result.ToList();
    }
}
