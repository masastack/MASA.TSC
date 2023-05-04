// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Teams;

public partial class Team
{
    private List<ProjectOverviewDto> _projects = new();
    private List<ProjectOverviewDto> _projectViewDatas;
    private AppMonitorDto _appMonitorDto = new();
    private MonitorStatuses _projectStatus;
    private bool _isLoading;
    private string? _projectType = "";
    private string? _search;
    private string? _hoverAppId;

    [Inject]
    public TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    public MasaUser MasaUser { get; set; } = default!;

    [Inject]
    public GlobalConfig GlobalConfig { get; set; } = default!;

    public MonitorStatuses ProjectStatus
    {
        get => _projectStatus;
        set
        {
            _projectStatus = value;
            GetProjectViewData();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        GlobalConfig.OnCurrentTeamChanged += TeamChange;
    }

    void TeamChange(Guid teamId)
    {
        MasaUser.CurrentTeamId = teamId;
        _ = InvokeAsync(async () =>
        {
            _isLoading = true;
            StateHasChanged();
            await GetProjectsAsync();
            StateHasChanged();
        });
    }

    private async Task GetProjectsAsync()
    {
        _isLoading = true;
        var data = await ApiCaller.ProjectService.OverviewAsync(new RequestTeamMonitorDto
        {
            EndTime = ConfigurationRecord.EndTime.UtcDateTime,
            StartTime = ConfigurationRecord.StartTime.UtcDateTime,
            Keyword = _search,
            UserId = CurrentUserId,
            TeamId = MasaUser.CurrentTeamId
        });
        _appMonitorDto = data?.Monitor ?? new();
        _projects = data?.Projects ?? new();
        GetProjectViewData();
        _isLoading = false;
    }

    IEnumerable<ProjectOverviewDto> GetProjectViewData()
    {
        IEnumerable<ProjectOverviewDto>? result = _projects;
        switch (_projectStatus)
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

        if (string.IsNullOrEmpty(_projectType) is false)
        {
            result = result.Where(item => item.LabelCode.Equals(_projectType, StringComparison.OrdinalIgnoreCase));
        }

        if (string.IsNullOrEmpty(_search) is false)
        {
            result = result.Where(item => item.Name.Contains(_search, StringComparison.OrdinalIgnoreCase) || item.Apps.Any(app => app.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)));
        }

        _projectViewDatas = result.ToList();

        return result;
    }

    private void OnProjectServiceClick(ProjectOverviewDto item, AppDto app)
    {
        ConfigurationRecord.ProjectId = item.Identity;
        ConfigurationRecord.TeamId = item.TeamId;
        ConfigurationRecord.TeamProjectCount = _projects.Count(p => p.TeamId == item.TeamId);
        ConfigurationRecord.TeamServiceCount = _projects.Where(p => p.TeamId == item.TeamId).Sum(p => p.Apps.Count);
        ConfigurationRecord.Service = app.Identity;
        ConfigurationRecord.TeamProjectDialogVisible = true;
    }

    async Task UpdateCardDataAsync()
    {
        var projects = GetProjectViewData();
        _appMonitorDto.ServiceTotal = projects.Count();
        _appMonitorDto.AppTotal = projects.Sum(project => project.Apps.Count);

        _appMonitorDto.ServiceWarn = projects.Count(project => project.Apps.Any(app => app.HasWarning));

        _appMonitorDto.ServiceError = projects.Count(project => project.Apps.Any(app => app.HasError));

        _appMonitorDto.Normal = projects.Count(project => project.Apps.All(app => !app.HasError && !app.HasWarning));
        _appMonitorDto.NormalAppTotal = projects.Sum(project => project.Apps.Count(app => !app.HasError && !app.HasWarning));

        var appids = string.Join(',', projects.Select(project => string.Join(',', project.Apps.Select(app => app.Identity)))).Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
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
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
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

    async Task ProjectTypeChangedAsync(string projectStype)
    {
        _projectType = projectStype;
        await UpdateCardDataAsync();
    }

    async Task OnSearchChangedAsync()
    {
        await GetProjectsAsync();
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await GetProjectsAsync();        
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        await OnDateTimeUpdateAsync(times);
        await InvokeAsync(StateHasChanged);
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

    private string ChipLabelClass(AppDto appDto)
    {
        return appDto.Status switch
        {
            MonitorStatuses.Warn => " cell-item-warn",
            MonitorStatuses.Error => " cell-item-error",
            _ => ""
        };
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        GlobalConfig.OnCurrentTeamChanged -= TeamChange;
    }

    private async Task SetAppId(string appid)
    {
        await Task.Delay(10);
        if (_hoverAppId != appid)
            _hoverAppId = appid;
    }
}
