﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

var configration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json")
            .Build();

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc(masaStackConfig.GetDefaultDccOptions());
});

var elasearchUrls = configration.GetSection("Masa:Elastic:Nodes").Get<string[]>();
var logIndexName = configration.GetSection("Masa:Elastic:logIndex").Get<string>();
var traceIndexName = configration.GetSection("Masa:Elastic:TraceIndex").Get<string>();
var prometheusUrl = configration.GetSection("Masa:Prometheus").Value;

builder.Services.AddElasticClientLogAndTrace(elasearchUrls, logIndexName, traceIndexName)
    .AddObservable(builder.Logging, new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = masaStackConfig.GetServerId(MasaStackConstant.TSC)
    }, masaStackConfig.OtlpUrl, false)
    .AddPrometheusClient(prometheusUrl)
    .AddTopology(elasearchUrls);

builder.Services.AddDaprClient();
builder.Services.AddDccClient();
var redisOption = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port=   masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword
};

RedisConfigurationOptions redis;
if (builder.Environment.IsDevelopment())
    redis = configration.GetSection("redis:RedisOptions").Get<RedisConfigurationOptions>();
else
    redis = redisOption;

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDaprStarter(opt =>
    {
        opt.DaprHttpPort = 3600;
        opt.DaprGrpcPort = 3601;
    });
}

builder.Services.AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = masaStackConfig.GetSsoDomain();
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
    .AddAuthClient(masaStackConfig.GetAuthServiceDomain(), redisOption)
    .AddPmClient(masaStackConfig.GetPmServiceDomain())
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
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
            .UseIntegrationEventBus(options =>
            {
                options.UseDapr()
                .UseEventLog<TscDbContext>()
                .UseEventBus(envenbusBuilder =>
                {
                    envenbusBuilder.UseMiddleware(typeof(DisabledCommandMiddleware<>));
                });
            })
            .UseUoW<TscDbContext>(dbOptions => dbOptions.UseSqlServer(masaStackConfig.GetConnectionString(AppSettings.Get("DBName"))).UseFilter())
            .UseRepository<TscDbContext>();
    })
    .AddTopologyRepository()
    .AddServices(builder);

await builder.Services.MigrateAsync();
app.UseMasaExceptionHandler(opt =>
{
    opt.ExceptionHandler = context =>
    {
        if (context.Exception is ValidationException validationException)
        {
            context.ToResult(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
        else if (context.Exception is UserStatusException userStatusException)
        {
            context.ToResult(userStatusException.GetLocalizedMessage(), 293);
        }
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
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