// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService(IServiceCollection services) : base(services, "/api/project")
    {
        App.MapGet($"{BaseUri}/overview", OverViewAsync);
        App.MapGet($"{BaseUri}", GetProjectsAsync);
    }

    private async Task<List<ProjectDto>> GetProjectsAsync([FromServices] IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new ProjectsQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<TeamMonitorDto> OverViewAsync([FromServices] IEventBus eventBus, RequestTeamMonitorDto model)
    {
        var teamQuery = new TeamMonitorQuery(model.UserId, model.ProjectId, model.StartTime, model.EndTime, model.Keyword);
        await eventBus.PublishAsync(teamQuery);
        return teamQuery.Result;
    }
}