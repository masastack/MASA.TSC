// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories.Topologies;

internal class TraceServiceStateRepository : ITraceServiceStateRepository
{
    private readonly IElasticClient _client;
    private readonly ILogger _logger;

    public TraceServiceStateRepository(ILogger<TraceServiceStateRepository> logger, IElasticClientFactory elasticClientFactory)
    {
        _client = elasticClientFactory.Create(TopologyConstants.ES_CLINET_NAME);
        _logger = logger;
    }

    public async Task AddAsync(params TraceServiceState[] data)
    {
        await _client.BulkAllAsync(data, TopologyConstants.SERVICE_STATEDATA_INDEX_NAME, _logger);
    }

    public async Task<List<TopologyServiceDataDto>> GetServiceTermsDataAsync(DateTime start, DateTime end, params string[] services)
    {
        var length = services.Length;
        var response = await _client.SearchAsync<TraceServiceState>(search => search
        .Query(q => q.DateRange(r => r.GreaterThanOrEquals(start).LessThanOrEquals(end).Field(f => f.Timestamp))).Size(0)
          .Index(TopologyConstants.SERVICE_STATEDATA_INDEX_NAME)
          .Aggregations(agg => agg.Terms(nameof(TraceServiceState.ServiceId), f => f.Field(f => f.ServiceId).Size(length)
                                                     .Aggregations(agg2 => agg2.Terms(nameof(TraceServiceState.DestServiceId), f2 => f2.Field(f2 => f2.DestServiceId).Size(length)
                                                                                                      .Aggregations(agg3 => agg3.ValueCount("CallCount", f => f.Field(f => f.ServiceId))
                                                                                                                                                      //.Sum("CallErrorCount", f=>f.Script(sc=>sc.Source("if(!doc['IsSuccess'].value){1}").))
                                                                                                                                                      .Average("AvgLatency", f => f.Field(f => f.Latency))
                                                                                                                                                      )
                                                     )))));

        if (!response.IsValid)
            if (response.OriginalException != null) throw response.OriginalException;
            else if (response.TryGetServerErrorReason(out string error)) throw new UserFriendlyException(error);
        if (!response.Aggregations.Any() || !response.Aggregations.ContainsKey(nameof(TraceServiceState.ServiceId)))
            return default!;

        var serviceGroups = (BucketAggregate)response.Aggregations[nameof(TraceServiceState.ServiceId)];
        return SetServiceId(serviceGroups);
    }

    private List<TopologyServiceDataDto> SetServiceId(BucketAggregate aggResult)
    {
        var result = new List<TopologyServiceDataDto>();
        foreach (var item in aggResult.Items)
        {
            var bucket = (KeyedBucket<object>)item;
            if (!bucket.ContainsKey(nameof(TraceServiceState.DestServiceId)))
                continue;

            var items = SetDesctServiceId((string)bucket.Key, (BucketAggregate)bucket[nameof(TraceServiceState.DestServiceId)]);
            if (items != null && items.Any())
                result.AddRange(items);
        }
        return result;
    }

    private IEnumerable<TopologyServiceDataDto> SetDesctServiceId(string serviceId, BucketAggregate aggResult)
    {
        var result = new List<TopologyServiceDataDto>();
        foreach (var item in aggResult.Items)
        {
            var ddd = (KeyedBucket<object>)item;
            var model = new TopologyServiceDataDto { CurrentId = serviceId, DestId = (string)ddd.Key };
            SetValues(ddd, model);
            result.Add(model);
        }
        return result;
    }

    private void SetValues(KeyedBucket<object> result, TopologyServiceDataDto model)
    {
        foreach (var key in result.Keys)
        {
            var value = result[key] as ValueAggregate;
            if (value == null)
                continue;
            if (key == "CallCount")
                model.CallsCount = Convert.ToInt32(value.Value);
            else if (key == "AvgLatency")
                model.AvgLatency = (int)Math.Floor(Convert.ToDouble(value.Value));
        }
    }
}
