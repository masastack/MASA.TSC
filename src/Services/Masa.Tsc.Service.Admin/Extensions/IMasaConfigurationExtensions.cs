// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

internal static class IMasaConfigurationExtensions
{
    private static IConfiguration GetConfiguration(IMasaConfiguration configuration, IMasaStackConfig masaStackConfig) => configuration.ConfigurationApi.Get(masaStackConfig.GetServiceId(MasaStackConstant.TSC));

    public static int[] GetTraceErrorStatus(this IMasaConfiguration configuration, IMasaStackConfig masaStackConfig)
    {
        var ports = GetConfiguration(configuration, masaStackConfig).GetSection(ConfigConst.TraceErrorPort).Get<int[]>();
        if (ports == null || !ports.Any())
            ports = new int[] { 500 };
        return ports;
    }

    public static (string logIndex, string traceIndex) GetElasticsearchLogTraceIndex(this IMasaConfiguration configuration, IMasaStackConfig masaStackConfig)
    {
        var config = GetConfiguration(configuration, masaStackConfig);
        string logIndex = config.GetSection(ConfigConst.LogIndex).Get<string>(),
            traceIndex = config.GetSection(ConfigConst.TraceIndex).Get<string>();
        return (logIndex, traceIndex);
    }
}
