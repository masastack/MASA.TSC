// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Topologies;

public class QueryHandler
{
    private readonly IMultilevelCacheClient _multilevelCacheClient;
    private readonly ITraceServiceStateRepository _traceServiceStateRepository;

    public QueryHandler(
        IMasaStackConfig masaStackConfig,
        IMultilevelCacheClientFactory multilevelCacheClientFactory,
        ITraceServiceStateRepository traceServiceStateRepository)
    {
        _multilevelCacheClient = multilevelCacheClientFactory.Create(masaStackConfig.GetServerId(MasaStackConstant.TSC));
        _traceServiceStateRepository = traceServiceStateRepository;
    }

    [EventHandler]
    public async Task GetTopologyAsync(TopologyQuery query)
    {
        var result = new TopologyResultDto();
        var services = (await _multilevelCacheClient.GetAsync<List<TraceServiceCache>>(TopologyConstants.TOPOLOGY_SERVICES_KEY))!;
        var service = services?.FirstOrDefault(m => m.Service == query.Data.ServiceName);
        if (service == null)
        {
            query.Result = result;
            return;
        }

        var relations = _multilevelCacheClient.Get<List<TraceServiceRelation>>(TopologyConstants.TOPOLOGY_SERVICES_RELATIONS_KEY)!;
        if (relations != null && relations.Any())
        {
            var findRelations = GetRelations(query.Data.Level, service.Id, relations);

            //所关联的服务id
            var serviceIds = GetServiceIds(findRelations);
            result.Services = services!.Where(item => serviceIds.Contains(item.Id)).Select(item => new TopologyServiceDto
            {
                Id = item.Id,
                Name = item.Service,
                Type = item.Type
            }).ToList();
            result.Relations = relations.Select(item => new TopologyServiceRelationDto { CurrentId = item.ServiceId, DestId = item.DestServiceId }).ToList();
            if (serviceIds.Any())
            {
                result.Data = await _traceServiceStateRepository.GetServiceTermsDataAsync(query.Data.Start, query.Data.End, serviceIds.ToArray());
                if (result.Data != null && result.Data.Any())
                {
                    var notExists = result.Relations.Where(r => !result.Data.Any(item => item.DestId == r.DestId && item.CurrentId == r.CurrentId)).ToList();
                    if (notExists.Any())
                        result.Data.AddRange(notExists.Select(r => new TopologyServiceDataDto
                        {
                            DestId = r.DestId,
                            CurrentId = r.CurrentId,
                        }));
                }
            }

            if (result.Data == null || !result.Data.Any())
            {
                result.Data = result.Relations.Select(r => new TopologyServiceDataDto
                {
                    DestId = r.DestId,
                    CurrentId = r.CurrentId,
                }).ToList();
            }

            query.Result = result;
        }
    }

    private List<TraceServiceRelation> GetRelations(int max, string serviceId, List<TraceServiceRelation> relations, int level = 1)
    {
        if (max - level < 0)
            return default!;
        var finds = relations.Where(m => m.DestServiceId == serviceId || m.ServiceId == serviceId).ToList();
        if (!finds.Any())
            return finds;

        var nextServiceIds = GetServiceIds(finds, serviceId);
        //relations.RemoveAll(m => m.DestServiceId == serviceId || m.ServiceId == serviceId);
        if (max - level == 0)
            return finds;
        foreach (var id in nextServiceIds)
        {
            var children = GetRelations(max, id, relations, level + 1);
            if (children != null && children.Any())
                finds.AddRange(children);
        }
        return finds;
    }

    private List<string> GetServiceIds(IEnumerable<TraceServiceRelation> data, string? currentId = null)
    {
        var result = new List<string>();
        foreach (var item in data)
        {
            if (!result.Contains(item.ServiceId))
                result.Add(item.ServiceId);
            if (!result.Contains(item.DestServiceId))
                result.Add(item.DestServiceId);
        }
        if (!string.IsNullOrEmpty(currentId))
            result.Remove(currentId);

        return result;
    }
}