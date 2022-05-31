// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService(IServiceCollection services) : base(services, "/api/project")
    {
        App.MapGet($"{BaseUri}/over-view", OverViewAsync);
        App.MapGet($"{BaseUri}", GetProjectsAsync);
    }

    private async Task<List<ProjectDto>> GetProjectsAsync([FromServices] IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new ProjectsQuery
        {
            UserId = userId
        };
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<TeamMonitorDto> OverViewAsync([FromServices] IEventBus eventBus, [FromQuery] RequestTeamMonitorDto model)
    {
        var teamQuery = new TeamMonitorQuery
        {
            EndTime = model.EndTime,
            StartTime = model.StartTime,
            Keyword = model.Keyword,
            ProjectId = model.ProjectId,
            UserId = model.UserId
        };
        await eventBus.PublishAsync(teamQuery);
        return teamQuery.Result;
    }
}