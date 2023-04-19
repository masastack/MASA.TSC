// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public static class ServiceExtensions
{
    internal const string DEFAULT_CLIENT_NAME = "masa.tsc.apigateways.caller";

    public static IServiceCollection AddTscApiCaller(this IServiceCollection services)
    {
        try
        {
            var caller = services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            if (caller != null)
                return services;
        }
        catch
        {
            IServiceProvider serviceProviderCopy = services.BuildServiceProvider();
            IMasaStackConfig masaStackConfig = serviceProviderCopy.GetRequiredService<IMasaStackConfig>();
            services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.UseDapr(options => options.AppId = masaStackConfig.GetServerId(MasaStackConstant.TSC),
                    builder => builder.UseDaprApiToken(serviceProviderCopy.GetRequiredService<TokenProvider>()?.AccessToken));
            });

            services.AddScoped(serviceProvider =>
            {
                serviceProviderCopy = serviceProvider;
                var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
                var client = new TscCaller(serviceProvider, caller);
                return client;
            });
        }

        return services;
    }
}
