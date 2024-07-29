// Copyright (c) MASA Stack All rights reserved.
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
        if (ConfigConst.StorageSetting.HasInit || !ConfigConst.IsClickhouse) return;
        if (string.IsNullOrEmpty(ConfigConst.ClikhouseConnection)) return;
        _services.AddMASAStackApmClickhouse(ConfigConst.ClikhouseConnection, ConfigConst.ClickhouseTableSuffix, ConfigConst.ClickHouseLogSourceTable, ConfigConst.ClickHouseTaceSourceTable
            , appLogSourceTable: ConfigConst.ClickHouseAppLogSourceTable, AppTraceSourceTable: ConfigConst.ClickHouseAppTraceSourceTable);
        ConfigConst.StorageSetting.SetClickhouse();
    }

    private static void AddElasticSearch()
    {
        if (ConfigConst.StorageSetting.HasInit || !ConfigConst.IsElasticsearch) return;
        string[] elasticsearchUrls = _services.GetMasaStackConfig().ElasticModel.Nodes?.ToArray() ?? Array.Empty<string>();
        _services.AddElasticClientLogAndTrace(elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
        ConfigConst.StorageSetting.SetElasticSearch();
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