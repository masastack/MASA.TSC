﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public static class ServiceExtensions
{
    internal const string DEFAULT_CLIENT_NAME = "masa.tsc.apigateways.caller";

    public static IServiceCollection AddTscApiCaller(this IServiceCollection services, string tscApiUrl)
    {
        try
        {
            var caller = services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            if (caller != null)
                return services;
        }
        catch
        {
            services.AddCaller(builder =>
            {
                builder.UseHttpClient(DEFAULT_CLIENT_NAME, options =>
                 {
                     options.BaseAddress = tscApiUrl;
                 });
            });

            services.AddScoped(serviceProvider =>
            {
                var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
                var client = new TscCaller(serviceProvider, caller, serviceProvider.GetRequiredService<TokenProvider>());
                return client;
            });
        }

        return services;
    }
}
