// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Dcc;
using Masa.Tsc.Web.Admin.Rcl.Extentions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton(sp => builder.Configuration);

await builder.Services.AddMasaStackConfigAsync(builder.Configuration);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<WasmRoutes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var masaStackConfig = builder.Services.GetMasaStackConfig();

MasaOpenIdConnectOptions masaOpenIdConnectOptions = new()
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackProject.TSC),
    Scopes = new List<string> { "openid", "profile" }
};

await builder.AddMasaOpenIdConnectAsync(masaOpenIdConnectOptions);

builder.Services.AddHttpClient("analysis", client =>
{
    var token = builder.Configuration.GetValue<string>("CUBE_JWT_TOKEN");
    client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
    client.DefaultRequestHeaders.Add("X-Request-Type", "GraphQL");
});

builder.Services.AddMasaStackComponent(MasaStackProject.TSC, $"{builder.HostEnvironment.BaseAddress}i18n");
builder.Services.AddRcl();
builder.Services.AddTscHttpApiCaller(masaStackConfig.GetTscServiceDomain(), masaStackConfig.GetAuthServiceDomain()).AddDccClient(masaStackConfig.GetDccServiceDomain());
var host = builder.Build();
await host.Services.InitializeMasaStackApplicationAsync();
await host.RunAsync();