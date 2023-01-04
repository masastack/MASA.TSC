// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TopologyService : ServiceBase
{
    public TopologyService() : base("/api/topology")
    {
        App.MapGet($"{BaseUri}/Start", StartAsync);
    }

    public async Task StartAsync([FromServices] IEventBus eventBus, [FromQuery] DateTimeOffset excuteTime)
    {
        var end = excuteTime.ToUniversalTime().DateTime;
        var start = DateTime.MinValue;
        var command = new StartCommand(start, end);
        await eventBus.PublishAsync(command);
    }

    public async Task<TopologyResultDto> GetAsync([FromServices] IEventBus eventBus, string serviceName, int level, DateTime start, DateTime end)
    {
        var query = new TopologyQuery(new TopologyRequestDto { ServiceName = serviceName, Level = level, Start = start, End = end });
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task StopAsync()
    {
        await Task.CompletedTask;
    }
}