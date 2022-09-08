// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class TeamService : BaseService
{
    public TeamService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/team",tokenProvider)
    {
    }

    public async Task<TeamDto> GetTeamAsync(Guid teamId, string projectId)
    {
        return await Caller.GetAsync<TeamDto>($"{RootPath}/{teamId}/{projectId}") ?? default!;
    }
}
