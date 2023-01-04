// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class TopologyService : BaseService
{
    public TopologyService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/topology", tokenProvider) { }

    public async Task<TopologyResultDto> GetAsync(string serviceName, int level, DateTime start, DateTime end) => (await Caller.GetAsync<TopologyResultDto>($"{RootPath}", new { serviceName, level, start, end }))!;
}
