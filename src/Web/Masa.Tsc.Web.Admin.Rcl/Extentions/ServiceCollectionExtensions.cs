// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Service.Caller;
using Microsoft.Extensions.DependencyInjection;

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRcl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<DashboardConfigurationRecord>();
        serviceCollection.AddScoped<TeamDetailConfigurationRecord>();
        serviceCollection.AddScoped<SearchData>();
        serviceCollection.AddTransient<TscGridstackJSModule>();
        return serviceCollection;
    }
}
