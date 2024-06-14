// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public static class ServiceExtensions
{
    internal const string DEFAULT_CLIENT_NAME = "masa.tsc.apigateways.caller";
    internal const string AUTH_CLIENT_NAME = "masa.tsc.auth.apigateways.caller";

    public static IServiceCollection AddTscApiCaller(this IServiceCollection services, string appid, string authAppid)
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
            services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.UseDapr(options => options.AppId = appid,
                    builder => builder.UseDaprApiToken(serviceProviderCopy.GetRequiredService<TokenProvider>()?.AccessToken));
            }).AddCaller(AUTH_CLIENT_NAME, builder =>
            {
                builder.UseDapr(options => options.AppId = authAppid,
                    builder => builder.UseDaprApiToken(serviceProviderCopy.GetRequiredService<TokenProvider>()?.AccessToken));
            });

            services.AddScoped(serviceProvider =>
            {
                serviceProviderCopy = serviceProvider;
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = new TscCaller(serviceProvider.GetRequiredService<IDccClient>(), callerFactory);
                return client;
            });
        }

        return services;
    }

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
            IServiceProvider serviceProviderCopy = services.BuildServiceProvider();
            services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.UseHttpClient(options =>
                {
                    options.BaseAddress = tscApiUrl;
                    options.Configure = (http) =>
                    {
                        var httpContext = serviceProviderCopy.GetRequiredService<IHttpContextAccessor>();
                        if (httpContext.HttpContext == null)
                            return;
                        var token = httpContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(token))
                            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    };
                });
            })
            .AddCaller(AUTH_CLIENT_NAME, builder =>
                {
                    builder.UseHttpClient(options =>
                    {
                        options.BaseAddress = authApiUrl;
                        options.Configure = (http) =>
                        {
                            var httpContext = serviceProviderCopy.GetRequiredService<IHttpContextAccessor>();
                            if (httpContext.HttpContext == null)
                                return;
                            var token = httpContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken).ConfigureAwait(false).GetAwaiter().GetResult();
                            if (!string.IsNullOrEmpty(token))
                                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        };
                    });
                });

            services.AddScoped(serviceProvider =>
            {
                serviceProviderCopy = serviceProvider;
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = new TscCaller(serviceProvider.GetRequiredService<IDccClient>(), callerFactory);
                return client;
            });
        }

        return services;
    }
}
