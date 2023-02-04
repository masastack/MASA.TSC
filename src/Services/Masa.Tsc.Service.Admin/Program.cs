﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

var elasearchUrls = builder.Configuration.GetSection("Masa:Elastic:nodes").Get<string[]>();
builder.Services.AddElasticClientLogAndTrace(elasearchUrls, builder.Configuration.GetSection("Masa:Elastic:logIndex").Get<string>(), builder.Configuration.GetSection("Masa:Elastic:traceIndex").Get<string>())
    .AddObservable(builder.Logging, builder.Configuration, false)
    .AddPrometheusClient(builder.Configuration.GetSection("Masa:Prometheus").Value,10)
    .AddTopology(elasearchUrls);
builder.Services.AddDaprClient();
var dccConfig = builder.Configuration.GetSection("Masa:Dcc").Get<DccOptions>();

RedisConfigurationOptions redis;
if (builder.Environment.IsDevelopment())
    redis = builder.Configuration.GetSection("redis:RedisOptions").Get<RedisConfigurationOptions>();
else
    redis = dccConfig.RedisOptions;

builder.Services.AddHttpContextAccessor()
    .AddMasaConfiguration(configurationBuilder => configurationBuilder.UseDcc(dccConfig, default, default));
IConfiguration config = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();

#if DEBUG
//builder.Services.AddDaprStarter(opt =>
//{
//    opt.DaprHttpPort = 3600;
//    opt.DaprGrpcPort = 3601;
//});
#endif

builder.Services.AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = config.GetValue<string>("$public.AppSettings:IdentityServerUrl");
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.MapInboundClaims = false;
    });
builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
    options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
})
    .AddScoped(service =>
    {
        var content = service.GetRequiredService<IHttpContextAccessor>();
        if (content.HttpContext != null && AuthenticationHeaderValue.TryParse(content.HttpContext.Request.Headers.Authorization.ToString(), out var auth) && auth != null)
            return new TokenProvider { AccessToken = auth?.Parameter };
        return default!;
    })
    .AddAuthClient(config["$public.AppSettings:AuthClient:Url"], dccConfig.RedisOptions)
    .AddPmClient(config["$public.AppSettings:PmClient:Url"])
    .AddMultilevelCache(MasaStackConsts.TSC_SYSTEM_SERVICE_APP_ID,
        distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(redis),
        multilevelCacheOptions =>
        {
            multilevelCacheOptions.SubscribeKeyPrefix = MasaStackConsts.TSC_SYSTEM_ID;
            multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey;
        });

var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    })
    .AddTransient(typeof(IMiddleware<>), typeof(LogMiddleware<>))
    .AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
                .UseIntegrationEventBus(options =>
                {
                    options.UseDapr()
                    .UseEventLog<TscDbContext>()
                    .UseEventBus();
                })
                .UseUoW<TscDbContext>(dbOptions => dbOptions.UseSqlServer().UseFilter())
                .UseRepository<TscDbContext>();
        })
    .AddTopologyRepository()
    .AddServices(builder);

await builder.Services.MigrateAsync();
app.UseMasaExceptionHandler();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});
app.UseHttpsRedirection();
app.Run();