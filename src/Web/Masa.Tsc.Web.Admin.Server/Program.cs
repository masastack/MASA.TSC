// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Configuration;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Provider;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservable();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc(sectionName: "Masa:Dcc");
});

var publicConfiguration = builder.GetMasaConfiguration().ConfigurationApi.GetPublic();
var oidc = builder.GetMasaConfiguration().Local.GetSection("Masa:Oidc").Get<MasaOpenIdConnectOptions>();
string authUrl = builder.GetMasaConfiguration().Local.GetValue<string>("Masa:Auth:ServiceBaseAddress");
string mcUrl = builder.GetMasaConfiguration().Local.GetValue<string>("Masa:Mc:ServiceBaseAddress");
builder.Services.AddMasaStackComponentsForServer("wwwroot/i18n", authUrl, mcUrl)
    .AddMasaOpenIdConnect(oidc);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<TscCaller>();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();