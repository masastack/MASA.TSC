// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories.Topologies;

public class TraceServiceRelationRepository : ITraceServiceRelationRepository
{
    private readonly IElasticClient _client;
    private readonly ILogger _logger;

    public TraceServiceRelationRepository(ILogger<TraceServiceRelationRepository> logger, IElasticsearchFactory elasticsearchFactory)
    {
        _client = elasticsearchFactory.CreateElasticClient(TopologyConstants.ES_CLINET_NAME);
        _logger = logger;
    }

    public async Task AddAsync(params TraceServiceRelation[] data)
    {
        await _client.BulkAllAsync(data, TopologyConstants.SERVICE_RELATION_INDEX_NAME, _logger);
    }

    public async Task<IEnumerable<TraceServiceRelation>> GetAllAsync()
    {
        return await _client.ScrollAllAsync<TraceServiceRelation>(TopologyConstants.SERVICE_RELATION_INDEX_NAME, "1m");
    }
}
