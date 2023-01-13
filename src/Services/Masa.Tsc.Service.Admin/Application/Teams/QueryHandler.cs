// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Teams;

public class QueryHandler
{
    private readonly IAuthClient _authClient;
    private readonly IPmClient _pmClient;
    private readonly ILogService _logService;
    private readonly ITraceService _traceService;

    public QueryHandler(IPmClient pmClient,
        IAuthClient authClient,
        ILogService logService,
        ITraceService traceService)
    {
        _authClient = authClient;
        _pmClient = pmClient;
        _logService = logService;
        _traceService = traceService;
    }

    [EventHandler]
    public async Task GetTeamDetailAsync(TeamDetailQuery query)
    {
        var result = await _authClient.TeamService.GetDetailAsync(query.TeamId);
        if (result != null)
        {
            query.Result = new TeamDto
            {
                Id = result.Id,
                Name = result.Name,
                Avatar = result.Avatar,
                Description = result.Description,
                Admins = result.Admins.Select(ToUser).ToList()
            };
            await SetProjectAsync(query);
        }
    }

    private async Task SetProjectAsync(TeamDetailQuery query)
    {
        var projects = await _pmClient.ProjectService.GetListByTeamIdsAsync(new List<Guid> { query.TeamId });
        if (projects != null && projects.Any())
        {
            query.Result.ProjectTotal = projects.Count;
            var project = projects.FirstOrDefault(p => p.Identity == query.ProjectId);
            if (project != null)
            {
                query.Result.CurrentProject = new ProjectDto
                {
                    Description = project.Description,
                    Identity = project.Identity,
                    Name = project.Name,
                    LabelName = project.LabelName,
                    Id = project.Id.ToString()
                };

                //需要优化，从列表可以返回创建人
                var detail = await _pmClient.ProjectService.GetAsync(project.Id);
                query.Result.CurrentProject.Creator = await GetUserAsync(detail.Creator);
                await SetAppAsync(query, projects);
            }
        }
    }

    private async Task<UserDto> GetUserAsync(Guid creatorId)
    {
        var creator = (await _authClient.UserService.GetUsersAsync(creatorId))?.First();
        if (creator != null)
            return new UserDto
            {
                Account = creator.Account,
                Avatar = creator.Avatar,
                DisplayName = creator.DisplayName,
                Gender = creator.Gender,
                Id = creator.Id,
                Name = creator.Name!,
            };
        return default!;
    }

    private async Task SetAppAsync(TeamDetailQuery query, List<Masa.BuildingBlocks.StackSdks.Pm.Model.ProjectModel> projects)
    {
        var apps = await _pmClient.AppService.GetListByProjectIdsAsync(projects.Select(p => p.Id).ToList());
        if (apps != null && apps.Any())
        {
            query.Result.AppTotal = apps.Count;
            query.Result.CurrentProject.Apps = apps.Where(a => a.ProjectId == Convert.ToInt32(query.Result.CurrentProject.Id)).Select(a => new AppDto
            {
                Id = a.Id.ToString(),
                Name = a.Name,
                Identity = a.Identity,
                ServiceType = a.ServiceType,
                AppType = a.Type
            }).ToList();
        }
    }

    [EventHandler]
    public async Task GetTeamMonitorAysnc(TeamMonitorQuery query)
    {
        var teams = await _authClient.TeamService.GetUserTeamsAsync();

        if (teams == null || !teams.Any())
            return;

        var monitors = await GetAllMonitServicesAsync(query.StartTime, query.EndTime);

        query.Result = new TeamMonitorDto
        {
            Projects = await GetAllProjects(teams.Select(t => t.Id).ToList(), monitors),
            Monitor = new AppMonitorDto()
        };

        var errors = await GetErrorOrWarnAsync(true, query.StartTime, query.EndTime);
        var warnings = await GetErrorOrWarnAsync(false, query.StartTime, query.EndTime);

        SetProjectErrorOrWarn(query, errors, true);
        SetProjectErrorOrWarn(query, warnings, false);
        query.Result.Projects = query.Result.Projects.Where(p => p.Apps != null && p.Apps.Any()).OrderBy(p=>p.Name).ToList();
        foreach (var project in query.Result.Projects)
        {
            project.Apps = project.Apps.OrderByDescending(app => app.Status).ThenBy(app => app.Name).ToList();
        }
        var appids = string.Join(",", query.Result.Projects.Select(p => string.Join(",", p.Apps.Select(app => app.Identity)))).Split(',');
        query.Result.Monitor.ServiceTotal = query.Result.Projects.Count;
        query.Result.Monitor.AppTotal = query.Result.Projects.Sum(p => p.Apps.Count);
        query.Result.Monitor.ServiceError = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Error);
        query.Result.Monitor.AppError = query.Result.Projects.Sum(p => p.Apps.Count(a => a.HasError));
        query.Result.Monitor.ServiceWarn = query.Result.Projects.Count(m => m.HasWarning);
        query.Result.Monitor.AppWarn = query.Result.Projects.Sum(p => p.Apps.Count(a => a.HasError));
        query.Result.Monitor.Normal = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Normal);
        query.Result.Monitor.ErrorCount = await GetErrorOrWarnAsync(true, appids, query.StartTime, query.EndTime);
        query.Result.Monitor.WarnCount = await GetErrorOrWarnAsync(false, appids, query.StartTime, query.EndTime);
    }

    private static void SetProjectErrorOrWarn(TeamMonitorQuery query, List<string> appIds, bool isError)
    {
        if (appIds == null || !appIds.Any())
            return;

        foreach (var project in query.Result.Projects)
        {
            if (project.Apps == null || !project.Apps.Any())
                continue;
            SetAppStatus(appIds, isError, project.Apps);
            SetProjectStatus(project, isError);
        }
    }

    private static void SetAppStatus(List<string> appids, bool isError, List<AppDto> apps)
    {
        foreach (var app in apps)
        {
            if (appids.Contains(app.Identity))
            {
                if (isError)
                {
                    app.Status = MonitorStatuses.Error;
                    app.HasError = true;
                }
                else
                {
                    if (!app.HasError)
                        app.Status = MonitorStatuses.Warn;
                    app.HasWarning = true;
                }
            }
        }
    }

    private static void SetProjectStatus(ProjectOverviewDto project, bool isError)
    {
        if (isError && project.Apps.Any(app => app.HasError))
        {
            project.Status = MonitorStatuses.Error;
            project.HasError = true;
        }
        else if (!isError && project.Apps.Any(app => app.HasWarning))
        {
            project.HasWarning = true;
            if (!project.HasError)
                project.Status = MonitorStatuses.Warn;
        }
    }

    private async Task<List<ProjectOverviewDto>> GetAllProjects(List<Guid> teamids, List<string> appids)
    {
        var result = new List<ProjectOverviewDto>();
        if (appids == null || !appids.Any())
            return result;

        var list = new List<int>();

        var projects = await _pmClient.ProjectService.GetListByTeamIdsAsync(teamids);
        if (projects == null || !projects.Any())
            return result;
        var apps = await _pmClient.AppService.GetListByProjectIdsAsync(projects.Select(p => p.Id).ToList());

        foreach (var project in projects)
        {
            if (list.Contains(project.Id))
                continue;

            var model = new ProjectOverviewDto
            {
                Id = project.Id.ToString(),
                Description = project.Description,
                Identity = project.Identity,
                LabelName = project.LabelName,
                Name = project.Name,
                TeamId = project.TeamId
            };

            var projectAppIds = apps.Where(a => a.ProjectId == project.Id).Select(app => app.Identity).ToList();
            if (!projectAppIds.Any())
                continue;
            if (appids.Any(id => projectAppIds.Contains(id)))
                model.Apps = apps.Where(a => a.ProjectId == project.Id).Select(a => new AppDto
                {
                    Id = a.Id.ToString(),
                    Identity = a.Identity,
                    Name = a.Name,
                    ServiceType = a.ServiceType,
                    AppType = a.Type
                }).ToList();

            result.Add(model);
        }

        return result;
    }

    private async Task<List<string>> GetAllMonitServicesAsync(DateTime? start = default, DateTime? end = default)
    {
        var logServices = (IEnumerable<string>)await _logService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = ElasticConstant.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.GroupBy,
            MaxCount = 999
        });

        var traceServices = (IEnumerable<string>)await _traceService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = ElasticConstant.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.GroupBy,
            MaxCount = 999
        });

        var result = new List<string>();
        if (logServices != null && logServices.Any())
            result.AddRange(logServices);
        if (traceServices != null && traceServices.Any())
            result.AddRange(traceServices);
        return result.Distinct().ToList();
    }

    private async Task<List<string>> GetErrorOrWarnAsync(bool isError, DateTime? start = default, DateTime? end = default)
    {
        var obj = await _logService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = ElasticConstant.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.GroupBy,
            MaxCount = 999,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{
                    Name="SeverityText",
                     Type= ConditionTypes.Equal,
                     Value= isError?"Error":"Warning"
                }
            }
        });
        var services = (IEnumerable<string>)obj;
        return services?.ToList()!;
    }

    private async Task<long> GetErrorOrWarnAsync(bool isError, IEnumerable<string> appids, DateTime? start = default, DateTime? end = default)
    {
        var obj = await _logService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = ElasticConstant.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.Count,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{
                    Name="SeverityText",
                     Type= ConditionTypes.Equal,
                     Value= isError?"Error":"Warning"
                },
                new FieldConditionDto
                {
                    Name=ElasticConstant.ServiceName,
                    Type= ConditionTypes.In,
                    Value=appids
                }
            }
        });
        return Convert.ToInt64(obj);
    }

    private UserDto ToUser(StaffModel model)
    {
        return new UserDto
        {
            Id = model.Id,
            Name = model.Name,
            DisplayName = model.DisplayName,
            Avatar = model.Avatar,
            Account = model.Account,
            Gender = model.Gender
        };
    }
}