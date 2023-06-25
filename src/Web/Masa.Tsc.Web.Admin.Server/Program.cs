// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<JsonOptions>(option =>
{
    option.JsonSerializerOptions.Converters.Add(new QueryResultDataResponseConverter());
});

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TLS_NAME")))
        {
            options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN");
        }
        else
        {
            options.ServerCertificate = X509Certificate2.CreateFromPemFile("./ssl/tls.crt", "./ssl/tls.key");
        }
        options.CheckCertificateRevocation = false;
    });
});

builder.Services.AddObservable(builder.Logging, new MasaObservableOptions
{
    ServiceNameSpace = builder.Environment.EnvironmentName,
    ServiceVersion = masaStackConfig.Version,
    ServiceName = masaStackConfig.GetWebId(MasaStackConstant.TSC),
    Layer = masaStackConfig.Namespace,
    ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")
}, masaStackConfig.OtlpUrl, true);

MasaOpenIdConnectOptions masaOpenIdConnectOptions = new()
{
    Authority = masaStackConfig.GetSsoDomain(),
    ClientId = masaStackConfig.GetWebId(MasaStackConstant.TSC),
    Scopes = new List<string> { "offline_access" }
};

IdentityModelEventSource.ShowPII = true;
builder.Services.AddMasaOpenIdConnect(masaOpenIdConnectOptions);

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

#if DEBUG
builder.AddMasaStackComponentsForServer(authHost: "https://auth-service-dev.masastack.com", mcHost: "https://mc-service-dev.masastack.com", pmHost: "https://pm-service-dev.masastack.com");
builder.Services.AddTscHttpApiCaller("http://localhost:18010").AddDccClient(redisOption);
#else
    builder.AddMasaStackComponentsForServer();
    builder.Services.AddTscHttpApiCaller(masaStackConfig.GetTscServiceDomain()).AddDccClient(redisOption);
#endif

builder.Services.AddRcl().AddScoped<TokenProvider>();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDaprStarter(opt =>
    {
        opt.AppId = masaStackConfig.GetWebId(MasaStackConstant.TSC);
        opt.DaprHttpPort = 3602;
        opt.DaprGrpcPort = 3603;
    });
}
builder.Services.AddDaprClient();

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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();