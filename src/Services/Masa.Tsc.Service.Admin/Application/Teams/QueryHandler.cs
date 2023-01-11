﻿// Copyright (c) MASA Stack All rights reserved.
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

        query.Result.Monitor.ServiceTotal = query.Result.Projects.Count;
        query.Result.Monitor.AppTotal = query.Result.Projects.Sum(p => p.Apps.Count);
        query.Result.Monitor.ServiceError = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Error);
        query.Result.Monitor.AppError = query.Result.Projects.Where(p => p.Status == MonitorStatuses.Error).Sum(p => p.Apps.Count(a => a.Status == MonitorStatuses.Error));
        query.Result.Monitor.ServiceWarn = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Warn);
        query.Result.Monitor.AppWarn = query.Result.Projects.Where(p => p.Status == MonitorStatuses.Warn).Sum(p => p.Apps.Count(a => a.Status == MonitorStatuses.Warn));
        query.Result.Monitor.Normal = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Normal);

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
                    app.Status = MonitorStatuses.Error;
                else if (app.Status == MonitorStatuses.Normal)
                    app.Status = MonitorStatuses.Warn;
            }
        }
    }

    private static void SetProjectStatus(ProjectOverviewDto project, bool isError)
    {
        if (isError && project.Apps.Any(app => app.Status == MonitorStatuses.Error))
            project.Status = MonitorStatuses.Error;
        else if (!isError && project.Status == MonitorStatuses.Normal && project.Apps.Any(app => app.Status == MonitorStatuses.Warn))
            project.Status = MonitorStatuses.Warn;
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

            if (apps != null && apps.Any())
            {
                model.Apps = apps.Where(a => a.ProjectId == project.Id && appids.Contains(a.Identity)).Select(a => new AppDto
                {
                    Id = a.Id.ToString(),
                    Identity = a.Identity,
                    Name = a.Name,
                    ServiceType = a.ServiceType,
                    AppType = a.Type
                }).ToList();
            }

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
      var obj=  await _logService.AggregateAsync(new SimpleAggregateRequestDto
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
                     Value= isError?"Error":"Warn"
                }
            }
        });
        var services = (IEnumerable<string>)obj;
        return services?.ToList()!;
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