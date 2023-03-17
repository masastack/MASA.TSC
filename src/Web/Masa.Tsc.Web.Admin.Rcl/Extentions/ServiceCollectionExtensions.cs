// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Models;

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRcl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ConfigurationRecord>();

        return serviceCollection;
    }
}
