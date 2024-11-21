// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class TopologyService : ServiceBase
{
    public TopologyService() : base("/api/topology")
    {
        App.MapGet($"{BaseUri}/start", StartAsync);
    }

    public async Task StartAsync([FromServices] IEventBus eventBus, [FromQuery] DateTimeOffset? excuteTime)
    {
        await Task.CompletedTask;
    }

    public async Task<TopologyResultDto> GetAsync([FromServices] IEventBus eventBus, string serviceName, int level, string start, string end)
    {
        var query = new TopologyQuery(new TopologyRequestDto { ServiceName = serviceName, Level = level, Start = start.ParseUTCTime(), End = end.ParseUTCTime() });
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task StopAsync()
    {
        await Task.CompletedTask;
    }
}