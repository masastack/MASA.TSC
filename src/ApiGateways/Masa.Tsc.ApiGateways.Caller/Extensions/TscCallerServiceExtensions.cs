// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class TscCallerServiceExtensions
{
    internal const string DEFAULT_CLIENT_NAME = "masa.tsc.apigateways.caller";
    internal const string AUTH_CLIENT_NAME = "masa.tsc.auth.apigateways.caller";

    //public static IServiceCollection AddTscApiCaller(this IServiceCollection services, string appid, string authAppid)
    //{
    //    try
    //    {
    //        var caller = services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
    //        if (caller != null)
    //            return services;
    //    }
    //    catch
    //    {
    //        IServiceProvider serviceProviderCopy = services.BuildServiceProvider();
    //        services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
    //        {
    //            builder.UseDapr(options => options.AppId = appid,
    //                builder => builder.UseDaprApiToken(serviceProviderCopy.GetRequiredService<TokenProvider>()?.AccessToken));
    //        }).AddCaller(AUTH_CLIENT_NAME, builder =>
    //        {
    //            builder.UseDapr(options => options.AppId = authAppid,
    //                builder => builder.UseDaprApiToken(serviceProviderCopy.GetRequiredService<TokenProvider>()?.AccessToken));
    //        });

    //        services.AddScoped(serviceProvider =>
    //        {
    //            serviceProviderCopy = serviceProvider;
    //            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
    //            var client = new TscCaller(serviceProviderCopy, callerFactory);
    //            return client;
    //        });
    //    }

    //    return services;
    //}

    public static IServiceCollection AddTscHttpApiCaller(this IServiceCollection services, string tscApiUrl, string authApiUrl)
    {
        try
        {
            var caller = services.BuildServiceProvider().GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            if (caller != null)
                return services;
        }
        catch
        {
            services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.UseHttpClient(options => options.BaseAddress = tscApiUrl).UseAuthentication();
            })
            .AddCaller(AUTH_CLIENT_NAME, builder =>
                {
                    builder.UseHttpClient(options => options.BaseAddress = authApiUrl).UseAuthentication();
                });

            services.AddScoped(serviceProvider =>
            {
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = new TscCaller(serviceProvider, callerFactory);
                return client;
            });
        }

        return services;
    }
}
