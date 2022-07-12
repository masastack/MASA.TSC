// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Extensions;

public static class ServiceExtensions
{
    private const string DEFAULT_CLIENT_NAME = "masa.stack.tsc.web.caller";

    public static IServiceCollection AddTscApiCaller(this IServiceCollection services, string tscApiUrl)
    {
        try
        {
            var caller = services.BuildServiceProvider().GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            if (caller != null)
                return services;
        }
        catch
        {

            services.AddCaller(builder =>
            {
                builder.UseHttpClient(options =>
                {
                    options.BaseAddress = tscApiUrl;
                    options.Name = DEFAULT_CLIENT_NAME;
                });
            });

            services.AddSingleton(serviceProvider =>
            {
                var caller = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
                var client = new TscCaller(caller);
                return client;
            });
        }

        return services;
    }
}
