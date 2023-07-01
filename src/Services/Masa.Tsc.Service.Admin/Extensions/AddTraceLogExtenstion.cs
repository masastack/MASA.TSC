// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AddTraceLogExtenstion
{
    private static IServiceCollection? _services;
    private static string[] _elasticsearchUrls;

    public static IServiceCollection AddTraceLog(this IServiceCollection services, string[] elasticsearchUrls)
    {
        _services = services;
        _elasticsearchUrls = elasticsearchUrls;
        var client = services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        try
        {
            var config = client.GetAsync<AppSettingConfiguration>(ConfigConst.ConfigRoot, ValueChanged).ConfigureAwait(false).GetAwaiter().GetResult();
            ConfigConst.SetConfiguration(config);
        }
        catch { }
        return services.AddElasticClientLogAndTrace(elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
    }

    private static void ValueChanged(AppSettingConfiguration config)
    {
        ConfigConst.SetConfiguration(config);
        _services?.AddElasticClientLogAndTrace(_elasticsearchUrls, ConfigConst.LogIndex, ConfigConst.TraceIndex);
    }
}