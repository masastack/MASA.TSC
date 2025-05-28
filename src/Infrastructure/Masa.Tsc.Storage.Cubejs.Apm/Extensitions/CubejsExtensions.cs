// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class CubejsExtensions
{
    internal const string Cubejs_Client_Name = "tsc.cubejs.http.client";

    public static IServiceCollection AddCubeApmService(this IServiceCollection service, string host, string token)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(host);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(token);
        service.AddHttpClient(Cubejs_Client_Name, client =>
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("X-Request-Type", "GraphQL");
        });
        var serializer = new SystemTextJsonSerializer(options =>
        {

        });
        service.AddKeyedScoped(Cubejs_Client_Name, (service, obj) => new GraphQLHttpClient(host, serializer, service.GetRequiredService<IHttpClientFactory>().CreateClient(obj!.ToString()!)));
        service.AddKeyedScoped<IApmService>(Cubejs_Client_Name, (service, obj) => new CubejsApmService(service.GetRequiredKeyedService<GraphQLHttpClient>(obj!.ToString()!)));
        return service;
    }

    public static IApmService GetCubeApmService(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredKeyedService<IApmService>(Cubejs_Client_Name);
    }

    public static ValueTuple<string, string> GetCubeSetting(this IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetRequiredKeyedService<GraphQLHttpClient>(Cubejs_Client_Name);
        return ValueTuple.Create(client.Options.EndPoint!.ToString(), client.HttpClient.DefaultRequestHeaders.Authorization!.Parameter!);
    }
}
