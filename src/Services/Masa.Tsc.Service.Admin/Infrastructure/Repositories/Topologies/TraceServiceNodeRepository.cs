// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories.Topologies;

public class TraceServiceNodeRepository : ITraceServiceNodeRepository
{
    private readonly IElasticClient _client;
    private readonly ILogger _logger;

    public TraceServiceNodeRepository(ILogger<TraceServiceNodeRepository> logger, IElasticsearchFactory elasticsearchFactory)
    {
        _client = elasticsearchFactory.CreateElasticClient(TopologyConstants.ES_CLINET_NAME);
        _logger = logger;
    }

    public async Task AddAsync(params TraceServiceNode[] data)
    {
        await _client.BulkAllAsync(data, TopologyConstants.SERVICE_INDEX_NAME, _logger);
    }

    public async Task<IEnumerable<TraceServiceNode>> GetAllAsync()
    {
        return await _client.ScrollAllAsync<TraceServiceNode>(TopologyConstants.SERVICE_INDEX_NAME, "1m");
    }
}
