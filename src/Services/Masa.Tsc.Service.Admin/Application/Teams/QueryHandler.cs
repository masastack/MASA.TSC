// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Teams;

public class QueryHandler : EnvQueryHandler
{
    private readonly IAuthClient _authClient;
    private readonly IPmClient _pmClient;
    private readonly ILogService _logService;
    private readonly ITraceService _traceService;
    private readonly IMasaPrometheusClient _prometheusClient;

    public QueryHandler(IPmClient pmClient,
        IAuthClient authClient,
        ILogService logService,
        IMasaPrometheusClient prometheusClient,
        ITraceService traceService,
        IMasaStackConfig masaStackConfig,
        IWebHostEnvironment environment,
        IMultiEnvironmentContext multiEnvironment
        ) : base(masaStackConfig, environment, multiEnvironment)
    {
        _authClient = authClient;
        _pmClient = pmClient;
        _logService = logService;
        _traceService = traceService;
        _prometheusClient = prometheusClient;
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
        var projects = await _pmClient.ProjectService.GetListByTeamIdsAsync(new List<Guid> { query.TeamId }, _multiEnvironment.CurrentEnvironment);
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
                    LabelCode = project.LabelCode,
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
        var creator = await _authClient.UserService.GetByIdAsync(creatorId);
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

        if (teams == null || !teams.Any(team => team.Id == query.TeamId))
            return;

        teams = teams.Where(team => team.Id == query.TeamId).ToList();
        var env = GetServiceEnvironmentName(string.Empty);
        var monitors = await GetAllMonitServicesAsync(env, query.StartTime, query.EndTime);

        query.Result = new TeamMonitorDto
        {
            Projects = await GetAllProjects(teams.First().Id, monitors),
            Monitor = new AppMonitorDto()
        };
        var appids = string.Join(",", query.Result.Projects.Select(p => string.Join(",", p.Apps.Select(app => app.Identity)))).Split(',').Where(s => s.Length > 0).ToArray();

        var (errors, warnings) = await GetProjectErrorAndWarnAsync(appids, ConfigConst.TraceErrorStatus, env, query.StartTime, query.EndTime);

        SetProjectErrorOrWarn(query, errors, true);
        SetProjectErrorOrWarn(query, warnings, false);
        query.Result.Projects = query.Result.Projects.Where(p => p.Apps != null && p.Apps.Any()).OrderBy(p => p.Name).ToList();
        foreach (var project in query.Result.Projects)
        {
            project.Apps = project.Apps.OrderByDescending(app => app.Status).ThenBy(app => app.Name).ToList();
        }

        query.Result.Monitor.ServiceTotal = query.Result.Projects.Count;
        query.Result.Monitor.AppTotal = query.Result.Projects.Sum(p => p.Apps.Count);
        query.Result.Monitor.ServiceError = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Error);
        query.Result.Monitor.AppError = query.Result.Projects.Sum(p => p.Apps.Count(a => a.HasError));
        query.Result.Monitor.ServiceWarn = query.Result.Projects.Count(m => m.HasWarning);
        query.Result.Monitor.AppWarn = query.Result.Projects.Sum(p => p.Apps.Count(a => a.HasError));
        query.Result.Monitor.Normal = query.Result.Projects.Count(m => m.Status == MonitorStatuses.Normal);
        query.Result.Monitor.NormalAppTotal = query.Result.Projects.Sum(m => m.Apps.Count(a => !a.HasError && !a.HasWarning));

        var (errorCount, warnCount) = await GetErrorAndWarnCountAsync(appids, ConfigConst.TraceErrorStatus, env, query.StartTime, query.EndTime);
        query.Result.Monitor.ErrorCount = errorCount;
        query.Result.Monitor.WarnCount = warnCount;
    }

    [EventHandler]
    public async Task GetAppErrorCountAsync(AppErrorCountQuery query)
    {
        var env = GetServiceEnvironmentName(query.AppId);
        query.Result = await GetErrorOrWarnCountAsync(new string[] { query.AppId }, ConfigConst.TraceErrorStatus, env, query.Start, query.End);
    }

    private async Task<(List<string> errorAppids, List<string> warningAppids)> GetProjectErrorAndWarnAsync(IEnumerable<string> appids, int[] errorPorts, string env, DateTime start, DateTime end)
    {
        var errorApps = await GetErrorOrWarnAsync(appids, errorPorts, env, start, end);
        return (errorApps, new List<string>());
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

    private async Task<List<ProjectOverviewDto>> GetAllProjects(Guid teamId, List<string> appids)
    {
        var result = new List<ProjectOverviewDto>();
        if (appids == null || !appids.Any())
            return result;

        var projects = await _pmClient.ProjectService.GetListByTeamIdsAsync(new List<Guid> { teamId }, _multiEnvironment.CurrentEnvironment);
        if (projects == null || !projects.Any())
            return result;
        var apps = await _pmClient.AppService.GetListByProjectIdsAsync(projects.Select(p => p.Id).ToList());

        foreach (var project in projects)
        {
            var model = new ProjectOverviewDto
            {
                Id = project.Id.ToString(),
                Description = project.Description,
                Identity = project.Identity,
                LabelName = project.LabelName,
                LabelCode = project.LabelCode,
                Name = project.Name,
                TeamId = teamId,
                Status = MonitorStatuses.Normal
            };

            var projectAppIds = apps.Where(a => a.ProjectId == project.Id).Select(app => app.Identity).ToList();
            if (!projectAppIds.Any())
                continue;
            if (appids.Exists(id => projectAppIds.Contains(id)))
                model.Apps = apps.Where(a => a.ProjectId == project.Id).Select(a => new AppDto
                {
                    Id = a.Id.ToString(),
                    Identity = a.Identity,
                    Name = a.Name,
                    ServiceType = a.ServiceType,
                    AppType = a.Type,
                    Status = MonitorStatuses.Normal
                }).ToList();

            result.Add(model);
        }

        return result;
    }

    private async Task<List<string>> GetAllMonitServicesAsync(string env, DateTime? start = default, DateTime? end = default)
    {
        if (start == null)
            start = DateTime.MinValue;
        if (end == null)
            end = DateTime.MaxValue;

        var query = new SimpleAggregateRequestDto
        {
            Name = StorageConst.Current.ServiceName,
            Start = start.Value,
            End = end.Value,
            Type = AggregateTypes.GroupBy,
            MaxCount = 999
        };
        query.SetEnv(env);

        var tasks = new Task<object>[] {
            _logService.AggregateAsync(query),
            _traceService.AggregateAsync(query)
        };
        var queryResult = await Task.WhenAll(tasks);

        var result = new List<string>();
        if (queryResult[0] is IEnumerable<string> logServices && logServices.Any())
            result.AddRange(logServices);
        if (queryResult[0] is IEnumerable<string> traceServices && traceServices.Any())
            result.AddRange(traceServices);

        var metricServices = await _prometheusClient.QueryRangeAsync(new QueryRangeRequest
        {
            Query = $"group by(service_name) (http_server_duration_sum{{{MetricConstants.Environment}=\"{env}\"}})",
            Start = start!.Value.ToUnixTimestamp().ToString(),
            End = end!.Value.ToUnixTimestamp().ToString(),
            Step = (end!.Value - start!.Value).TotalSeconds.ToString(),
        });
        if (metricServices.Status == ResultStatuses.Success && metricServices.Data!.Result != null && metricServices.Data.Result.Any() && metricServices.Data.ResultType == ResultTypes.Vector)
        {
            result.AddRange(metricServices.Data!.Result.Select(item => ((QueryResultInstantVectorResponse)item).Metric!["service_name"].ToString()!));
        }
        return result.Distinct().ToList();
    }

    private async Task<List<string>> GetErrorOrWarnAsync(IEnumerable<string> appids, int[] errorPorts, string env, DateTime? start = default, DateTime? end = default)
    {
        var obj = await _traceService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = StorageConst.Current.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.GroupBy,
            MaxCount = 999,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{
                    Name=StorageConst.Current.Environment,
                    Type=ConditionTypes.Equal,
                    Value  =env
                },
                new FieldConditionDto{
                    Name=StorageConst.Current.Trace.HttpStatusCode,
                     Type= ConditionTypes.In,
                     Value=errorPorts.Select(num=>(object)num)
                },
                new FieldConditionDto{
                    Name=StorageConst.Current.ServiceName,
                     Type= ConditionTypes.In,
                     Value=appids
                }
            }
        });
        var services = (IEnumerable<string>)obj;
        return services?.ToList()!;
    }

    private async Task<(long errorCount, long warningCount)> GetErrorAndWarnCountAsync(IEnumerable<string> appids, int[] errorPorts, string env, DateTime start, DateTime end)
    {
        var errorCount = await GetErrorOrWarnCountAsync(appids, errorPorts, env, start, end);
        return (errorCount, 0);
    }

    private async Task<long> GetErrorOrWarnCountAsync(IEnumerable<string> appids, int[] errorPorts, string env, DateTime? start = default, DateTime? end = default)
    {
        if (appids == null || !appids.Any())
            return default;
        var obj = await _traceService.AggregateAsync(new SimpleAggregateRequestDto
        {
            Name = StorageConst.Current.ServiceName,
            Start = start ?? DateTime.MinValue,
            End = end ?? DateTime.MinValue,
            Type = AggregateTypes.Count,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{
                    Name=StorageConst.Current.Environment,
                    Type=ConditionTypes.Equal,
                    Value  =env
                },
                new FieldConditionDto{
                    Name=StorageConst.Current.Trace.HttpStatusCode,
                    Type = ConditionTypes.In,
                    Value=errorPorts.Select(num=>(object)num)
                },
                new FieldConditionDto
                {
                    Name=StorageConst.Current.ServiceName,
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
