// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AddTraceLogExtenstion
{
    private static IServiceCollection _services;

    public static IServiceCollection AddTraceLog(this IServiceCollection services)
    {
        _services = services;
        // var client = _services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        //var config1 = client.GetAsync<AppSettingConfiguration>(ConfigConst.ConfigRoot, ValueChanged).ConfigureAwait(false).GetAwaiter().GetResult();



        var config = new AppSettingConfiguration
        {
            IsClickHouse = true,
            Clickhouse = new ClickhouseConfiguration
            {
                Connection = "Compress=True;CheckCompressedHash=False;Compressor=lz4;SocketTimeout=10000;Host=10.130.0.11;Port=9000;User=otel;Password=otel@prd;Database=otel_prd",
                AppLogTable = "otel_logs",
                AppTraceTable = "otel_traces",
                LogSource = "otel_logs_masa",
                TraceSource = "otel_traces_masa",
                Suffix = "masa_v1"
            },
            Trace = new AppSettingTraceConfiguration
            {
                ErrorStatus = new int[] {
                           400,
                           500,
                           501,
                           502,
                           503
                         }
            },
            Cubejs = new CubejsConfig {
                 Endpoint= "http://10.130.0.33:4000/cubejs-api/graphql",
                 Token= "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjo0Mn0.V8hDRVWs1NslqGrvv-rMcNX0VTC4ZCC3LMIDS72Z7rI"
            }
        };

        ConfigConst.SetConfiguration(config);

        AddClickHouse();
        AddElasticSearch();
        ArgumentNullException.ThrowIfNull(config.Cubejs);
        _services.AddCubeApmService(config.Cubejs.Endpoint, config.Cubejs.Token);
        return _services;
    }

    private static void AddClickHouse()
    {
        if (ConfigConst.StorageSetting.HasInit || !ConfigConst.IsClickhouse) return;
        if (string.IsNullOrEmpty(ConfigConst.ClikhouseConnection)) return;
        _services.AddMASAStackApmClickhouse(ConfigConst.ClikhouseConnection, ConfigConst.ClickhouseTableSuffix, ConfigConst.ClickHouseLogSourceTable, ConfigConst.ClickHouseTaceSourceTable
            , appLogSourceTable: ConfigConst.ClickHouseAppLogSourceTable, AppTraceSourceTable: ConfigConst.ClickHouseAppTraceSourceTable,
            ConfigConst.ClickHouseStoragePolicy, ConfigConst.ClickHouseTTLDays);
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