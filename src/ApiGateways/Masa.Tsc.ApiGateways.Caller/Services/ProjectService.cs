// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class ProjectService : BaseService
{
    public ProjectService(ICallerProvider caller) : base(caller, "/api/project") { }

    public async Task<List<ProjectDto>> GetProjectsAsync(Guid userId)
    {
        return await Caller.GetAsync<List<ProjectDto>>($"{RootPath}?userId={userId}") ?? default!;
    }

    public async Task<TeamMonitorDto> OverViewAsync(RequestTeamMonitorDto model)
    {
        return await Caller.GetAsync<TeamMonitorDto>($"{RootPath}/over-view", model) ?? default!;
    }
}
