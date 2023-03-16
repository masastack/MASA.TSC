// Copyright (c) MASA Stack All rights reserved.
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
            IServiceProvider serviceProviderCopy = services.BuildServiceProvider();
            services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.UseHttpClient(options =>
                 {
                     options.BaseAddress = tscApiUrl;
                     options.Configure = (http) =>
                     {
                         var token = serviceProviderCopy.GetRequiredService<TokenProvider>();
                         if (token != null && !string.IsNullOrEmpty(token.AccessToken))
                             http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                     };
                 });
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
