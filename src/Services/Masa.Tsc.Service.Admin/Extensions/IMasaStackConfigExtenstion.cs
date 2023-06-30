// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config;

public static class IMasaStackConfigExtenstion
{
    private static string[] serviceIds = default!;
    private static readonly object serviceLock = new();

    public static string GetServiceEnvironmentName(this IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, string serviceName, string environmentName)
    {
        if (string.IsNullOrEmpty(serviceName))
            return environmentName;
        var services = GetServiceIds(masaStackConfig);
        if (services.Contains(serviceName))
            return environment.EnvironmentName;
        return environmentName;
    }

    public static string GetServiceEnvironmentName(this IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IEnumerable<string> serviceNames, string environmentName)
    {
        if (serviceNames == null || !serviceNames.Any())
            return environmentName;
        var masaServices = GetServiceIds(masaStackConfig);
        bool isMasaService = false;
        foreach (var service in serviceNames)
        {
            if (!masaServices.Contains(service))
            {
                isMasaService = true;
                break;
            }
        }

        return isMasaService ? environment.EnvironmentName : environmentName;
    }

    private static MasaStackProject[] GetServices()
    {
        return new MasaStackProject[] {
        MasaStackProject.Auth,
        MasaStackProject.DCC,
        MasaStackProject.PM,
        MasaStackProject.MC,
        MasaStackProject.Scheduler,
        MasaStackProject.Alert,
        MasaStackProject.TSC
    };
    }

    private static string[] GetServiceIds(IMasaStackConfig masaStackConfig)
    {
        if (serviceIds != null)
            return serviceIds;
        lock (serviceLock)
        {
            var result = new List<string>() {
            masaStackConfig.GetId(MasaStackProject.Auth, MasaStackApp.SSO),
            masaStackConfig.GetId(MasaStackProject.Scheduler, MasaStackApp.Worker)
        };
            var services = GetServices();
            foreach (var service in services)
            {
                result.Add(masaStackConfig.GetServiceId(service));
                result.Add(masaStackConfig.GetWebId(service));
            }
            serviceIds = result.ToArray();
        }
        return serviceIds;
    }
}
