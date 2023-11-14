// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config;

public static class IMasaStackConfigExtenstion
{
    public static string GetServiceEnvironmentName(this IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, string serviceName, string environmentName)
    {
        return string.IsNullOrEmpty(environmentName) ? environment.EnvironmentName : environmentName;
    }

    public static string GetServiceEnvironmentName(this IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IEnumerable<string> serviceNames, string environmentName)
    {
        return GetServiceEnvironmentName(masaStackConfig, environment, serviceNames?.FirstOrDefault()!, environmentName);
    }
}
