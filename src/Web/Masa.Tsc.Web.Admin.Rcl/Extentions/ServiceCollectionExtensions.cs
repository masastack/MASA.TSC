// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRcl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<DashboardConfigurationRecord>();
        serviceCollection.AddScoped<TeamDetailConfigurationRecord>();
        serviceCollection.AddTransient<TscGridstackJSModule>();

        return serviceCollection;
    }
}
