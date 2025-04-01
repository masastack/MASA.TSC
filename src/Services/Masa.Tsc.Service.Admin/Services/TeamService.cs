// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class TeamService : ServiceBase
{
    public TeamService() : base("/api/team")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        App.MapGet($"{BaseUri}/{{teamId}}/{{projectId}}", GetAsync).RequireAuthorization();
    }

    private async Task<TeamDto> GetAsync([FromServices] IEventBus eventBus, Guid teamId, string projectId)
    {
        var teamQuery = new TeamDetailQuery(teamId, projectId);
        await eventBus.PublishAsync(teamQuery);
        return teamQuery.Result;
    }
}