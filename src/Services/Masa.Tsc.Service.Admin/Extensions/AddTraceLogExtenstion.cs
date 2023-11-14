// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AddTraceLogExtenstion
{
    private static IServiceCollection? _services;
    private static string[] _elasticsearchUrls;

    public static IServiceCollection AddTraceLog(this IServiceCollection services, IConfiguration configuration)
    {
        if (AddClickHouse(services, configuration))
            return services;
        string[] elasticsearchUrls = services.GetMasaStackConfig().ElasticModel.Nodes?.ToArray() ?? Array.Empty<string>();
        _services = services;
        _elasticsearchUrls = elasticsearchUrls;
        var client = services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        try
        {
            var config = client.GetAsync<AppSettingConfiguration>(ConfigConst.ConfigRoot, ValueChanged).ConfigureAwait(false).GetAwaiter().GetResult();
            ConfigConst.SetConfiguration(config);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return services.AddElasticClientLogAndTrace(elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
    }

    private static bool AddClickHouse(IServiceCollection services, IConfiguration configuration)
    {
        var clickhouseConnection = configuration.GetValue<string>("CLICKHOUSE_CONNECTIONSTRING");
        if (string.IsNullOrEmpty(clickhouseConnection))
            return false;
        var traceTable = configuration.GetValue<string>("CLICKHOUSE_TRACE_TABLE");
        var logTable = configuration.GetValue<string>("CLICKHOUSE_LOG_TABLE");
        services.AddMASAStackClickhouse(clickhouseConnection, logTable, traceTable);
        return true;
    }

    private static void ValueChanged(AppSettingConfiguration config)
    {
        ConfigConst.SetConfiguration(config);
        _services?.AddElasticClientLogAndTrace(_elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
    }
}