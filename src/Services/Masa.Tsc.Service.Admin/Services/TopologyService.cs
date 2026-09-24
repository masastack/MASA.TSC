// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models.Response;

namespace Masa.Tsc.Service.Admin.Services;

internal class TopologyService : ServiceBase
{
    public TopologyService() : base("/api/topology")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        App.MapGet($"{BaseUri}/start", StartAsync).RequireAuthorization();
    }

    public async Task StartAsync([FromServices] IEventBus eventBus, [FromQuery] DateTimeOffset? excuteTime)
    {
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<ServiceTopologiesDto>> GetServicesAsync([FromServices] IEventBus eventBus, [FromServices] IApmService apmService, string? service = default)
    {
        var data = (await apmService.GetTopologiesAsync()).ToList() ?? [];
        if (data == null || !data.Any())
        {
            return [];
        }
        if (!string.IsNullOrEmpty(service))
        {
            List<ServiceTopologiesDto> targets = [];
            var find = data.FirstOrDefault(x => string.Equals(x.Service, service, StringComparison.InvariantCultureIgnoreCase));
            if (find == null)
                return [];
            targets.Add(find);
            data.Remove(find);
            GetLoopChildren(find, data, targets);

            return targets;
        }
        return data;
    }

    private void GetLoopChildren(ServiceTopologiesDto find, List<ServiceTopologiesDto> all, List<ServiceTopologiesDto> targets)
    {
        do
        {
            var matches = GetClients(find.Servers, all);
            if (matches == null || !matches.Any())
                break;
            if (matches.All(m => targets.Exists(t => t.Service == m.Service)))
                break;

            var notExists = matches.Where(m => !targets.Exists(t => t.Service == m.Service));
            targets.AddRange(notExists);
            all.RemoveAll(x => notExists.Any(m => m.Service == x.Service));

            foreach (var child in notExists)
            {
                GetLoopChildren(child, all, targets);
            }
        } while (true);
    }

    private IEnumerable<ServiceTopologiesDto>? GetClients(IEnumerable<string> targets, IEnumerable<ServiceTopologiesDto> all)
    {
        if (targets == null || !targets.Any()) return default;
        return all.Where(x => targets.Contains(x.Service, StringComparer.InvariantCultureIgnoreCase));
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