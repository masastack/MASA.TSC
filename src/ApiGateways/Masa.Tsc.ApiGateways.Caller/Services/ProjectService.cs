// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class ProjectService : BaseService
{
    private readonly IDccClient _dccClient;

    public ProjectService(ICaller caller, TokenProvider tokenProvider, IDccClient dccClient) : base(caller, "/api/project", tokenProvider)
    {
        _dccClient = dccClient;
    }

    public async Task<List<ProjectDto>> GetProjectsAsync(Guid userId) => (await Caller.GetAsync<List<ProjectDto>>($"{RootPath}?userId={userId}"))!;

    public async Task<TeamMonitorDto> OverviewAsync(RequestTeamMonitorDto model) => (await Caller.GetAsync<TeamMonitorDto>($"{RootPath}/overview", model))!;

    public async Task<List<KeyValuePair<string, string>>> GetProjectTypesAsync()
    {
        var data = await _dccClient.LabelService.GetListByTypeCodeAsync("ProjectType");
        if (data == null || !data.Any())
            return new List<KeyValuePair<string, string>>();

        return data.Select(item => new KeyValuePair<string, string>(item.Code, item.Name)).ToList();
    }
}
