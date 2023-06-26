// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AddTraceLogExtenstion
{
    public static IServiceCollection AddTraceLog(this IServiceCollection services, IMasaStackConfig masaStackConfig, string[] elasticsearchUrls)
    {
        var configration = services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();
        (string logIndex, string traceIndex) = configration.GetElasticsearchLogTraceIndex(masaStackConfig);

        if (!string.IsNullOrEmpty(logIndex))
            ElasticSearchConst.SetLogIndex(logIndex);
        if (!string.IsNullOrEmpty(traceIndex))
            ElasticSearchConst.SetTraceIndex(traceIndex);

        return services.AddElasticClientLogAndTrace(elasticsearchUrls, ElasticSearchConst.LogIndex, ElasticSearchConst.TraceIndex);
    }
}