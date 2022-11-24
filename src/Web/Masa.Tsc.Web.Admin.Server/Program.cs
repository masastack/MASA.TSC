// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
using Masa.Tsc.Contracts.Admin.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddObservable();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<JsonOptions>(option =>
{
    option.JsonSerializerOptions.Converters.Add(new QueryResultDataResponseConverter());
});

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

var dccConfig = builder.Configuration.GetSection("Masa:Dcc").Get<DccOptions>();
IConfiguration config;
if (builder.Environment.EnvironmentName == "Development")
{
    builder.Services.AddMasaConfiguration(configurationBuilder =>
    {
        configurationBuilder.UseDcc(dccConfig, default, default);
    });
    config = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
}
else
{
    config = builder.Configuration;
}

var oidc = config.GetSection("$public.OIDC").Get<MasaOpenIdConnectOptions>();
string authUrl = config.GetValue<string>("$public.AppSettings:AuthClient:Url");
string mcUrl = config.GetValue<string>("$public.AppSettings:McClient:Url");
string pmUrl = config.GetValue<string>("$public.AppSettings:PmClient:Url");
builder.AddMasaStackComponentsForServer("wwwroot/i18n", authUrl, mcUrl, pmUrl).AddMasaOpenIdConnect(oidc);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<TscCaller>();
//builder.Services.AddScoped<AddInstrumentDto>();

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