// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = new();
    private List<ProjectOverviewDto> _viewProjects = new();
    private TeamSearchModel? _teamSearchModel = null;
    private AppMonitorDto _appMonitorDto;
    private bool _isLoad = true;
    private StringNumber _projectStatus = "0";

    private bool _visible;
    private ProjectOverviewDto _projectOverviewDto = default!;
    private TeamDialogModel teamDialog = new();

    private void HandleOnItemClick(ProjectOverviewDto item)
    {
        teamDialog.ProjectId = item.Identity;
        teamDialog.TeamId = item.TeamId;
        teamDialog.TeamProjectCount = _projects.Count(p => p.TeamId == item.TeamId);
        teamDialog.TeamServiceCount = _projects.Where(p => p.TeamId == item.TeamId).Sum(p => p.Apps.Count);
        teamDialog.Start = _teamSearchModel.Start.ToDateTimeOffset(CurrentTimeZone);
        teamDialog.End = _teamSearchModel.End.ToDateTimeOffset(CurrentTimeZone);
        _projectOverviewDto = item;
        _visible = true;
    }

    private void OnProjectServiceClick(ProjectOverviewDto item, string serviceId)
    {
        teamDialog.ProjectId = item.Identity;
        teamDialog.TeamId = item.TeamId;
        teamDialog.TeamProjectCount = _projects.Count(p => p.TeamId == item.TeamId);
        teamDialog.TeamServiceCount = _projects.Where(p => p.TeamId == item.TeamId).Sum(p => p.Apps.Count);
        teamDialog.Start = _teamSearchModel.Start.ToDateTimeOffset(CurrentTimeZone);
        teamDialog.End = _teamSearchModel.End.ToDateTimeOffset(CurrentTimeZone);
        teamDialog.ServiceId = serviceId;
        _projectOverviewDto = item;
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

    private string GetHexTitleClass(MonitorStatuses status)
    {
        return status switch
        {
            MonitorStatuses.Normal => "green--text",
            MonitorStatuses.Warn => "warning--text",
            MonitorStatuses.Error => "error--text",
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
        teamDialog.Start = _teamSearchModel.Start.ToDateTimeOffset(CurrentTimeZone);
        teamDialog.End = _teamSearchModel.End.ToDateTimeOffset(CurrentTimeZone);
        await LoadData();
    }

    private async Task OnSearchChangeAsync(TeamSearchModel query)
    {
        _teamSearchModel = query;
        teamDialog.Start = _teamSearchModel.Start.ToDateTimeOffset(CurrentTimeZone);
        teamDialog.End = _teamSearchModel.End.ToDateTimeOffset(CurrentTimeZone);
        _viewProjects = GetViewData();
        await UpdateCardDataAsync(_viewProjects);
    }

    async Task ProjectStatusChangedAsync(StringNumber projectStatus)
    {
        _projectStatus = projectStatus;
        _viewProjects = GetViewData();
        await Task.CompletedTask;
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

    async Task UpdateCardDataAsync(IEnumerable<ProjectOverviewDto> data)
    {
        _appMonitorDto.ServiceTotal = data.Count();
        _appMonitorDto.AppTotal = data.Sum(project => project.Apps.Count);

        _appMonitorDto.ServiceWarn = data.Count(project => project.Apps.Any(app => app.HasWarning));

        _appMonitorDto.ServiceError = data.Count(project => project.Apps.Any(app => app.HasError));

        _appMonitorDto.Normal = data.Count(project => project.Apps.All(app => !app.HasError && !app.HasWarning));

        var appids = string.Join(',', data.Select(project => string.Join(',', project.Apps?.Select(app => app.Identity)))).Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        var tasks = new Task<int>[] {
            GetErroOrWarningCountAsync(true,appids),
            GetErroOrWarningCountAsync(false,appids),
        };

        var counts = await Task.WhenAll(tasks);
        _appMonitorDto.ErrorCount = counts[0];
        _appMonitorDto.WarnCount = counts[1];
    }

    async Task<int> GetErroOrWarningCountAsync(bool isError, params string[] appids)
    {
        if (appids.Length == 0)
            return default;
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = _teamSearchModel.Start.Value,
            End = _teamSearchModel.End.Value,
            Name = ElasticSearchConst.ServiceName,
            Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                Name=ElasticSearchConst.LogLevelText,
                Type= ConditionTypes.Equal,
                Value=isError?ElasticSearchConst.LogErrorText:ElasticSearchConst.LogWarningText
                },
                new FieldConditionDto
                {
                    Name=ElasticSearchConst.ServiceName,
                    Type= ConditionTypes.In,
                    Value=appids
                }
            }
        };
        return await ApiCaller.LogService.AggregateAsync<int>(query);
    }

    DateTimeOffset ToDateTimeOffset(DateTime? time)
    {
        if (time == null)
            return DateTimeOffset.MinValue;
        return new DateTimeOffset(ticks: time.Value.Ticks + CurrentTimeZone.BaseUtcOffset.Ticks, offset: CurrentTimeZone.BaseUtcOffset);
    }
}
