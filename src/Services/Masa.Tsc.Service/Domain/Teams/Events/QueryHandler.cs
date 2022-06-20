// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Domain.Teams.Events;

public class QueryHandler
{
    private readonly IAuthClient _authClient;
    private readonly IPmClient _pmClient;
    private readonly IElasticClient _elasticClient;

    public QueryHandler(IPmClient pmClient, IAuthClient authClient, IElasticClient elasticClient)
    {
        _authClient = authClient;
        _pmClient = pmClient;
        _elasticClient = elasticClient;
    }

    [EventHandler]
    public async Task GetTeamDetailAsync(TeamDetailQuery query)
    {
        var result = await _authClient.TeamService.GetDetailAsync(query.Id);
        if (result != null)
        {
            var projects = await _pmClient.ProjectService.GetListByTeamIdAsync(query.Id);
            query.Result = new TeamDto
            {
                Id = result.Id,
                Name = result.Name,
                Avatar = result.Avatar,
                Description = result.Description,
                Admins = result.Admins.Select(ToUser).ToList(),
                ProjectTotal = projects?.Count ?? 0,
                CurrentAppId = query.Id
            };

            if (projects != null && projects.Any())
            {
                var apps = await _pmClient.AppService.GetListByProjectIdsAsync(projects.Select(p => p.Id).ToList());
                if (apps != null && apps.Any())
                {
                    query.Result.AppTotal = apps.Count;
                    int appid = Convert.ToInt32(query.AppId);
                    var app = apps.FirstOrDefault(a => a.Id == appid);
                    if (app != null)
                    {
                        var project = projects.FirstOrDefault(p => p.Id == app.ProjectId);
                        if (project != null)
                        {
                            query.Result.CurrentProject = new ProjectDto
                            {
                                Apps = apps.Where(a => a.ProjectId == project.Id).Select(a => new AppDto
                                {
                                    Id = a.Id.ToString(),
                                    Name = a.Name,
                                    Identity = a.Identity,
                                    ServiceType = (ServiceTypes)((int)a.ServiceType)
                                }).ToList(),
                                Description = project.Description,
                                Identity = project.Identity,
                                Name = project.Name,
                                LabelName = project.LabelName,
                                Id = project.Id.ToString()
                            };
                        }
                    }
                }
            }

        }
    }

    [EventHandler]
    public async Task GetTeamMonitorAysnc(TeamMonitorQuery query)
    {
        var teams = new TeamDetailModel[] { };
        if (teams == null || !teams.Any())
            return;
        query.Result = new TeamMonitorDto
        {
            Projects = await GetAllProjects(teams.Select(t => t.Id).ToList()),
            Monitor = new AppMonitorDto()
        };

        var monitors = await GetAllMonitor();
        var errorWarns = await GetErrorAndWarn();

        int error = 0, warn = 0, errorWarnAppCount = 0;
        if (errorWarns != null && errorWarns.Any())
        {
            foreach (var project in query.Result.Projects)
            {
                bool isError = false, isWarn = false;
                foreach (var app in project.Apps)
                {
                    if (errorWarns.ContainsKey(app.Identity))
                    {
                        var item = errorWarns[app.Identity];

                        if (item.Item2 > 0)
                        {
                            if (!isError && !isWarn)
                                isWarn = true;
                            app.Status = MonitorStatuses.Warn;
                            warn += item.Item2;
                        }

                        if (item.Item1 > 0)
                        {
                            if (!isError)
                                error += item.Item1;
                            app.Status = MonitorStatuses.Error;
                        }
                    }
                }
                if (isError)
                    project.Status = MonitorStatuses.Error;
                else if (isWarn)
                    project.Status = MonitorStatuses.Warn;
            }

            errorWarnAppCount = errorWarns.Where(item => item.Value.Item1 > 0 || item.Value.Item2 > 0).Count();
        }

        if (monitors != null && monitors.Any())
        {
            query.Result.Monitor.Total = monitors.Count;
            query.Result.Monitor.Error = error;
            query.Result.Monitor.Warn = warn;
            if (monitors.Count - errorWarnAppCount > 0)
            {
                query.Result.Monitor.Nomal = monitors.Count - errorWarnAppCount;
            }
        }

    }

    private async Task<List<ProjectOverViewDto>> GetAllProjects(List<Guid> teamids)
    {
        var list = new List<int>();
        var result = new List<ProjectOverViewDto>();
        foreach (var id in teamids)
        {
            var projects = await _pmClient.ProjectService.GetListByTeamIdAsync(id);
            if (projects == null || !projects.Any())
                continue;
            var apps = await _pmClient.AppService.GetListByProjectIdsAsync(projects.Select(p => p.Id).ToList());

            foreach (var project in projects)
            {
                if (list.Contains(project.Id))
                    continue;

                var model = new ProjectOverViewDto
                {
                    Id = project.Id.ToString(),
                    Description = project.Description,
                    Identity = project.Identity,
                    LabelName = project.LabelName,
                    Name = project.Name
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
        }

        return result;
    }

    private async Task<List<string>> GetAllMonitor()
    {
        await Task.CompletedTask;
        return new List<string> { "service1", "service2" };
    }

    private async Task<Dictionary<string, Tuple<int, int>>> GetErrorAndWarn()
    {
        await Task.CompletedTask;
        return new Dictionary<string, Tuple<int, int>> {
            { "service1",Tuple.Create(5,20)},
            { "service2",Tuple.Create(0,0)}
        };
    }

    private UserDto ToUser(StaffModel model)
    {
        return new UserDto
        {
            Id = model.Id,
            Name = model.User.Name,
            DisplayName = model.User.DisplayName,
            Avatar = model.User.Avatar,
            Account = model.User.Account,
            Gender = (GenderTypes)((int)model.User.Gender)
        };
    }
}