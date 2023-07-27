﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TopologyService : ServiceBase
{
    public TopologyService() : base("/api/topology")
    {
        App.MapGet($"{BaseUri}/start", StartAsync);
    }

    public async Task StartAsync([FromServices] IEventBus eventBus, [FromQuery] DateTimeOffset? excuteTime)
    {
        if (excuteTime == null)
            excuteTime = new DateTimeOffset(DateTime.UtcNow);
        var end = excuteTime.Value.ToUniversalTime().DateTime;
        var start = end.AddDays(-7);
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