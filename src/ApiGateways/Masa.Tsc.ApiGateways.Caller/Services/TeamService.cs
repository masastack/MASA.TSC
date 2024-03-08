// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public sealed class TeamService : BaseService
{
    internal TeamService(ICaller caller) : base(caller, "/api/team") { }

    public async Task<TeamDto> GetTeamAsync(Guid teamId, string projectId) => (await Caller.GetAsync<TeamDto>($"{RootPath}/{teamId}/{projectId}"))!;
}