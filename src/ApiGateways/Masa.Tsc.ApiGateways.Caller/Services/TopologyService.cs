// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public sealed class TopologyService : BaseService
{
    internal TopologyService(ICaller caller) : base(caller, "/api/topology") { }

    public async Task<TopologyResultDto> GetAsync(string? serviceName, int level, DateTime start, DateTime end) => (await Caller.GetAsync<TopologyResultDto>($"{RootPath}", new { serviceName, level, start, end }))!;
}
