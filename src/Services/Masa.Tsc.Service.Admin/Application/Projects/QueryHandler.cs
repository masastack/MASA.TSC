// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Projects;

public class QueryHandler
{
    private readonly IPmClient _pmClient;
    private readonly IAuthClient _authClient;
    private readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public QueryHandler(IPmClient pmClient, IAuthClient authClient, IMultiEnvironmentContext multiEnvironmentContext)
    {
        _pmClient = pmClient;
        _authClient = authClient;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler]
    public async Task GetProjectAsync(ProjectQuery query)
    {
        var project = await _pmClient.ProjectService.GetByIdentityAsync(query.ProjectId);
        if (project == null)
            throw new UserFriendlyException($"Project {query.ProjectId} is not exists");

        var apps = await _pmClient.AppService.GetListByProjectIdsAsync(new List<int> { project.Id });
        var team = await _authClient.TeamService.GetDetailAsync(project.TeamId)!;
        var creator = await _authClient.UserService.GetByIdAsync(project.Creator);

        query.Result = new ProjectDto
        {
            TeamId = team?.Id ?? Guid.Empty,
            Id = project.Identity,
            Description = project.Description,
            Identity = project.Identity,
            LabelCode = project.LabelCode,
            Name = project.Name
        };

        if (apps != null && apps.Any())
            query.Result.Apps = apps.Select(a => new AppDto { }).ToList();
        if (creator != null)
            query.Result.Creator = new UserDto
            {
                Account = creator.Account,
                Avatar = creator.Avatar,
                DisplayName = creator.DisplayName,
                Gender = creator.Gender,
                Id = creator.Id,
                Name = creator.Name!,
            };
    }

    [EventHandler]
    public async Task GetProjectsAsync(ProjectsQuery query)
    {
        var teams = await _authClient.TeamService.GetUserTeamsAsync();
        if (teams != null && teams.Any())
        {
            query.Result = await GetAllProjectsAsync(teams.Select(t => t.Id).ToList());
        }
    }

    private async Task<List<ProjectDto>> GetAllProjectsAsync(List<Guid> teamIds)
    {
        var result = new List<ProjectDto>();
        var list = new List<int>();

        var projects = await _pmClient.ProjectService.GetListByTeamIdsAsync(teamIds, _multiEnvironmentContext.CurrentEnvironment);
        if (projects == null || !projects.Any())
            return result;
        foreach (var project in projects)
        {
            if (list.Contains(project.Id))
                continue;

            list.Add(project.Id);
            result.Add(new ProjectDto
            {
                Id = project.Id.ToString(),
                Identity = project.Identity,
                Name = project.Name,
                Description = project.Description,
                LabelName = project.LabelName,
                LabelCode = project.LabelCode,
            });
        }

        return result;
    }

    [EventHandler]
    public async Task GetAppsAsync(AppsQuery query)
    {
        if (int.TryParse(query.ProjectId, out int projectId) && projectId > 0)
        {
            var result = await _pmClient.AppService.GetListByProjectIdsAsync(new List<int> { projectId });
            if (result != null && result.Any())
            {
                query.Result = result.Select(m => new AppDto
                {
                    Id = m.Id.ToString(),
                    Identity = m.Identity,
                    Name = m.Name,
                    ServiceType = m.ServiceType,
                    AppType = m.Type
                }).ToList();
            }
        }
    }
}
