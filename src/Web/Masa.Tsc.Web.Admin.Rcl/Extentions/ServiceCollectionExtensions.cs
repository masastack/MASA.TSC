// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Service;
using System.Net.Http.Headers;

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class RclServiceCollectionExtensions
{
    public static IServiceCollection AddRcl(this IServiceCollection serviceCollection, string cubeHost, string cubeToken)
    {
        serviceCollection.AddScoped<DashboardConfigurationRecord>();
        serviceCollection.AddScoped<TeamDetailConfigurationRecord>();
        serviceCollection.AddScoped<SearchData>();
        serviceCollection.AddTransient<TscGridstackJSModule>();
        serviceCollection.AddCubejsService(cubeHost, cubeToken);
        return serviceCollection;
    }

    internal const string Cubejs_Client_Name = "tsc.web.cubejs.http.client";

    public static IServiceCollection AddCubejsService(this IServiceCollection service, string host, string token)
    {
        //var host = "https://tsc-service-sec-staging.masastack.com/cubejs-api/graphql";
        //var token = "";
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
        //service.AddKeyedScoped<IApmService>(Cubejs_Client_Name, (service, obj) => new CubejsApmService(service.GetRequiredKeyedService<GraphQLHttpClient>(obj!.ToString()!)));
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


    public static GraphQL.Client.Http.GraphQLHttpClient GetCubejs(IServiceProvider s)
    {
        return s.GetRequiredKeyedService<GraphQLHttpClient>(Cubejs_Client_Name);
    }
}
