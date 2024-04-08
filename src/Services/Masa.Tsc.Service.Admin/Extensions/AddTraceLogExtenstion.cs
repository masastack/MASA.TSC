﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AddTraceLogExtenstion
{
    private static IServiceCollection _services;

    public static IServiceCollection AddTraceLog(this IServiceCollection services)
    {
        _services = services;
        var client = _services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        var config = client.GetAsync<AppSettingConfiguration>(ConfigConst.ConfigRoot, ValueChanged).ConfigureAwait(false).GetAwaiter().GetResult();
        ConfigConst.SetConfiguration(config);
        AddClickHouse();
        AddElasticSearch();
        return _services;
    }

    private static void AddClickHouse()
    {
        if (ConfigConst.StorageConst.HasInit || !ConfigConst.IsClickhouse) return;
        if (string.IsNullOrEmpty(ConfigConst.ClikhouseConnection)) return;
        _services.AddMASAStackApmClickhouse(ConfigConst.ClikhouseConnection, "test01", "otel_logs_new", "otel_traces_new");        
        ConfigConst.StorageConst.SetClickhouse();
    }

    private static void AddElasticSearch()
    {
        if (ConfigConst.StorageConst.HasInit || !ConfigConst.IsElasticsearch) return;
        string[] elasticsearchUrls = _services.GetMasaStackConfig().ElasticModel.Nodes?.ToArray() ?? Array.Empty<string>();
        _services.AddElasticClientLogAndTrace(elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
        ConfigConst.StorageConst.SetElasticSearch();
    }

    private static void ValueChanged(AppSettingConfiguration config)
    {
        ConfigConst.SetConfiguration(config);
        var logger = _services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger("Masa.Tsc.Service.Admin");
        logger.LogInformation("ValueChanged config value is:{config}", JsonSerializer.Serialize(config));
        //ConfigConst.StorageConst.Reset();
        //AddClickHouse();
        //AddElasticSearch();
    }
}