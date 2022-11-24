// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class ProjectService : BaseService
{
    public ProjectService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/project", tokenProvider) { }

    public async Task<List<ProjectDto>> GetProjectsAsync(Guid userId) => (await Caller.GetAsync<List<ProjectDto>>($"{RootPath}?userId={userId}"))!;

    public async Task<TeamMonitorDto> OverviewAsync(RequestTeamMonitorDto model) => (await Caller.GetAsync<TeamMonitorDto>($"{RootPath}/overview", model))!;
}
