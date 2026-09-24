// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries.Topologies;

internal class QueryHandler(IApmService apmService)
{
    readonly IApmService _apmService = apmService;

    [EventHandler]
    public void GetTopology(TopologyQuery query)
    {
        var result = new TopologyResultDto();
        query.Result = result;
    }

    [EventHandler]
    public async Task GetTopologySimple(TopologySimpleQuery query)
    {
        var data = (await _apmService.GetTopologiesAsync()).ToList() ?? [];
        if (data == null || !data.Any())
        {
            return;
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            List<ServiceTopologiesDto> targets = [];
            var find = data.FirstOrDefault(x => string.Equals(x.Service, query.Service, StringComparison.InvariantCultureIgnoreCase));
            if (find == null)
                return;
            targets.Add(find);
            data.Remove(find);
            GetLoopChildren(find, data, targets);
            query.Result = targets;
            return;
        }
        query.Result = data;
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
}