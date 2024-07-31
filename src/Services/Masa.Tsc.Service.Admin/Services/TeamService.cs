// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TeamService : ServiceBase
{
    public TeamService() : base("/api/team")
    {
        App.MapGet($"{BaseUri}/{{teamId}}/{{projectId}}", GetAsync);
    }

    private async Task<TeamDto> GetAsync([FromServices] IEventBus eventBus, Guid teamId, string projectId)
    {
        var teamQuery = new TeamDetailQuery(teamId, projectId);
        await eventBus.PublishAsync(teamQuery);
        return teamQuery.Result;
    }
}