// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

var elasearchUrls = AppSettings.GetModel<string[]>("Masa:Elastic:Nodes");
var logIndexName = AppSettings.Get("Masa:Elastic:logIndex");
var traceIndexName = AppSettings.Get("Masa:Elastic:TraceIndex");
var prometheusUrl = AppSettings.Get("Masa:Prometheus");

builder.Services.AddElasticClientLogAndTrace(elasearchUrls, logIndexName, traceIndexName)
    .AddObservable(builder.Logging, new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = masaStackConfig.GetServerId(MasaStackConstant.TSC)
    }, masaStackConfig.OtlpUrl, false)
    .AddPrometheusClient(prometheusUrl, 15)
    .AddTopology(elasearchUrls);

builder.Services.AddDaprClient();
await builder.Services.AddDccClient().AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain()).AddSchedulerJobAsync();
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
    redis = AppSettings.GetModel<RedisConfigurationOptions>("LocalRedisOptions");
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
    .AddMultilevelCache(masaStackConfig.GetServerId(MasaStackConstant.TSC),
        distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(redis),
        multilevelCacheOptions =>
        {
            multilevelCacheOptions.SubscribeKeyPrefix = MasaStackConstant.TSC;
            multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey;
        });

builder.Services.AddStackMiddleware().AddHealthChecks();

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
                .UseEventBus();
            })
            .UseUoW<TscDbContext>(dbOptions => dbOptions.UseSqlServer(masaStackConfig.GetConnectionString(AppSettings.Get("DBName"))).UseFilter())
            .UseRepository<TscDbContext>();
    })
    .AddTopologyRepository()
    .AddServices(builder);

app.UseAddStackMiddleware();
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
app.UseHealthChecks("/healthy");
app.Run();