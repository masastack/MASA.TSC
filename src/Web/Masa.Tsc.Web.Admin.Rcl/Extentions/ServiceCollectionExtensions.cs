// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRcl(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<Dictionary<Guid, UpsertChartPanelDto>>();

        return serviceCollection;
    }
}
