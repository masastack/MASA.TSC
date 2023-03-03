// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public static class IServiceCollectionExtensitions
{
    public static IServiceCollection AddTopology(this IServiceCollection services, string[] elasearchUrls)
    {
        services.AddElasticsearch(TopologyConstants.ES_CLINET_NAME, options =>
          {
              options.UseNodes(elasearchUrls).UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
          });

        var fatory = services.BuildServiceProvider().GetRequiredService<IElasticClientFactory>();
        var client = fatory.Create(TopologyConstants.ES_CLINET_NAME);
        var rep = client.Indices.Exists(TopologyConstants.SERVICE_INDEX_NAME);
        if (!rep.Exists)
            client.Indices.Create(TopologyConstants.SERVICE_INDEX_NAME, c => c.Map<TraceServiceNode>(m => m.AutoMap()));

        rep = client.Indices.Exists(TopologyConstants.SERVICE_RELATION_INDEX_NAME);
        if (!rep.Exists)
            client.Indices.Create(TopologyConstants.SERVICE_RELATION_INDEX_NAME, c => c.Map<TraceServiceRelation>(m => m.AutoMap()));

        rep = client.Indices.Exists(TopologyConstants.SERVICE_STATEDATA_INDEX_NAME);
        if (!rep.Exists)
            client.Indices.Create(TopologyConstants.SERVICE_STATEDATA_INDEX_NAME, c => c.Map<TraceServiceState>(m => m.AutoMap()));

        return services;
    }

    public static IServiceCollection AddTopologyRepository(this IServiceCollection services)
    {
        services.AddSingleton<ITraceServiceNodeRepository, TraceServiceNodeRepository>()
                    .AddSingleton<ITraceServiceRelationRepository, TraceServiceRelationRepository>()
                    .AddSingleton<ITraceServiceStateRepository, TraceServiceStateRepository>();
        //load data to cache

        return services;
    }
}