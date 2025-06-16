// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.TSC, MasaStackApp.WEB);
var masaStackConfig = builder.Services.GetMasaStackConfig();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRcl(builder.Configuration.GetValue<string>("CUBE_HOST")!, builder.Configuration.GetValue<string>("CUBE_JWT_TOKEN")!).AddScoped<TokenProvider>();

builder.Services.Configure<JsonOptions>(option =>
{
    option.JsonSerializerOptions.Converters.Add(new QueryResultDataResponseConverter());
});

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TLS_NAME")))
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
        }
        options.CheckCertificateRevocation = false;
    });
});
MasaOpenIdConnectOptions masaOpenIdConnectOptions = new()
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackProject.TSC),
    Scopes = new List<string> { "offline_access" },
    
};

IdentityModelEventSource.ShowPII = true;
builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions).AddMemoryCache();
var redisOption = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port= masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};
BlazorRouteManager.InitRoute(Assembly.Load("Masa.Tsc.Web.Admin.Rcl"));

#if DEBUG
if (builder.Environment.EnvironmentName == "Development")
{
    //await builder.Services.AddMasaStackComponentsAsync(MasaStackProject.TSC, authHost: "https://auth-service.lonsid.cn", mcHost: "https://mc-service.lonsid.cn", pmHost: "https://pm-service.lonsid.cn");
    await builder.Services.AddMasaStackComponentsWithNormalAppAsync(MasaStackProject.TSC, "http://localhost:4317", serviceVersion: "local1.0", authHost: "https://auth-service.lonsid.cn", mcHost: "https://mc-service.lonsid.cn", pmHost: "https://pm-service.lonsid.cn");
    builder.Services.AddTscHttpApiCaller("http://localhost:18010", authApiUrl: "https://auth-service.lonsid.cn").AddDccClient(redisOption);
}
else
{
    await builder.Services.AddMasaStackComponentsAsync(MasaStackProject.TSC);
    builder.Services.AddTscHttpApiCaller("http://localhost:18010", masaStackConfig.GetAuthServiceDomain()).AddDccClient(redisOption);
}
#else
    await builder.Services.AddMasaStackComponentsAsync(MasaStackProject.TSC);
    builder.Services.AddTscHttpApiCaller(masaStackConfig.GetTscServiceDomain(),masaStackConfig.GetAuthServiceDomain()).AddDccClient(redisOption);
#endif

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

//#if DEBUG
//builder.Services.AddDaprStarter(opt =>
//{
//    opt.AppId = masaStackConfig.GetWebId(MasaStackProject.TSC);
//    opt.DaprHttpPort = 3602;
//    opt.DaprGrpcPort = 3603;
//});
//#endif
//builder.Services.AddDaprClient();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();