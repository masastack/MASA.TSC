// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Application.Teams;

public class QueryHandler
{
    private readonly IAuthClient _authClient;
    private readonly IPmClient _pmClient;
    private readonly IElasticClient _elasticClient;
    private readonly IMasaPrometheusClient _prometheusClient;

    public QueryHandler(IPmClient pmClient, IAuthClient authClient, IElasticClient elasticClient, IMasaPrometheusClient prometheusClient)
    {
        _authClient = authClient;
        _pmClient = pmClient;
        _elasticClient = elasticClient;
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
                await SetAppAsync(query, projects);
            }
        }
    }

    private async Task SetAppAsync(TeamDetailQuery query, List<Masa.BuildingBlocks.BasicAbility.Pm.Model.ProjectModel> projects)
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
                ServiceType = (ServiceTypes)(int)a.ServiceType
            }).ToList();
        }
    }

    [EventHandler]
    public async Task GetTeamMonitorAysnc(TeamMonitorQuery query)
    {
        var teams = await _authClient.TeamService.GetUserTeamsAsync();

        if (teams == null || !teams.Any())
            return;
        query.Result = new TeamMonitorDto
        {
            Projects = await GetAllProjects(teams.Select(t => t.Id).ToList()),
            Monitor = new AppMonitorDto()
        };

        var monitors = await GetAllMonitorAsync();
        var errorWarns = await GetErrorAndWarnAsync();

        int error = 0, warn = 0, errorWarnAppCount = 0;
        if (errorWarns != null && errorWarns.Any())
        {
            SetProjectStatus(query, errorWarns, ref error, ref warn);
            errorWarnAppCount = errorWarns.Where(item => item.Value.Item1 > 0 || item.Value.Item2 > 0).Count();
        }

        query.Result.Monitor.Error = error;
        query.Result.Monitor.Warn = warn;
        if (monitors != null && monitors.Any())
        {
            query.Result.Monitor.Total = monitors.Count;
            if (monitors.Count - errorWarnAppCount > 0)
            {
                query.Result.Monitor.Normal = monitors.Count - errorWarnAppCount;
            }
        }
    }

    private static void SetProjectStatus(TeamMonitorQuery query, Dictionary<string, (int, int)> errorWarns, ref int error, ref int warn)
    {
        if (errorWarns == null || !errorWarns.Any())
            return;

        foreach (var project in query.Result.Projects)
        {
            bool isError = false, isWarn = false;
            foreach (var app in project.Apps)
            {
                SetAppStatus(errorWarns, ref error, ref warn, isError, ref isWarn, app);
            }
            if (isError)
                project.Status = MonitorStatuses.Error;
            else if (isWarn)
                project.Status = MonitorStatuses.Warn;
        }
    }

    private static void SetAppStatus(Dictionary<string, (int, int)> errorWarns, ref int error, ref int warn, bool isError, ref bool isWarn, AppDto app)
    {
        if (errorWarns.ContainsKey(app.Identity))
        {
            var item = errorWarns[app.Identity];

            if (item.Item2 > 0)
            {
                if (!isError && !isWarn)
                {
                    isWarn = true;
                    app.Status = MonitorStatuses.Warn;
                }
                warn += item.Item2;
            }

            if (item.Item1 > 0)
            {
                if (!isError)
                {
                    isError = true;
                    isWarn = false;
                    app.Status = MonitorStatuses.Error;
                }
                error += item.Item1;

            }
        }
    }

    private async Task<List<ProjectOverviewDto>> GetAllProjects(List<Guid> teamids)
    {
        var list = new List<int>();
        var result = new List<ProjectOverviewDto>();

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
                TeamId = teamids.FirstOrDefault()
            };

            if (apps != null && apps.Any())
            {
                model.Apps = apps.Where(a => a.ProjectId == project.Id).Select(a => new AppDto
                {
                    Id = a.Id.ToString(),
                    Identity = a.Identity,
                    Name = a.Name,
                    ServiceType = (ServiceTypes)((int)a.ServiceType)
                }).ToList();
            }

            result.Add(model);
        }

        return result;
    }

    private async Task<List<string>> GetAllMonitorAsync()
    {
        await Task.CompletedTask;
        return new List<string> { "service1", "service2" };
    }

    private async Task<Dictionary<string, ValueTuple<int, int>>> GetErrorAndWarnAsync()
    {
        await Task.CompletedTask;
        return new Dictionary<string, ValueTuple<int, int>> {
            { "service1",ValueTuple.Create(5,20)},
            { "service2",ValueTuple.Create(0,0)}
        };
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