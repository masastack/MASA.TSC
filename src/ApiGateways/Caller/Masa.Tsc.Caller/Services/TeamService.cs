// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Services;

public class TeamService : BaseService
{
    public TeamService(ICallerProvider caller) : base(caller, "/api/team")
    {
    }

    public async Task<TeamDto> GetTeamAsync(string appId)
    {
        return await Caller.GetAsync<TeamDto>($"{RootPath}?appId={appId}") ?? default!;
    }
}
