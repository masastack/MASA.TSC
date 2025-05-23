﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.TSC, MasaStackApp.Service, callerAction: callerAction =>
{
    callerAction.UseAuthentication();
});
var masaStackConfig = builder.Services.GetMasaStackConfig();
var prometheusUrl = builder.Configuration.GetValue<string>("Prometheus")!;
var appid = masaStackConfig.GetServiceId(MasaStackProject.TSC);
var envAppid = $"{appid}_{masaStackConfig.Environment}";
var connStr = masaStackConfig.GetValue(MasaStackConfigConstant.CONNECTIONSTRING);
var dbModel = JsonSerializer.Deserialize<DbModel>(connStr)!;
bool isPgsql = string.Equals(dbModel.DbType, "postgresql", StringComparison.CurrentCultureIgnoreCase);

builder.Services.AddTraceLog()
    .AddObservable(builder.Logging, new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = appid,
        Layer = masaStackConfig.Namespace,
        ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")!
    },
#if DEBUG
       "http://localhost:4317"
#else
        masaStackConfig.OtlpUrl
#endif
    )
    .AddPrometheusClient(prometheusUrl, 15)
    .AddCors(options =>
    {
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    })
    .AddAuthorization()
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
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
    });

builder.Services.AddIsolation(services => services.UseMultiEnvironment());
builder.Services.AddDaprClient();
builder.Services.AddMemoryCache();

//开启response stream读取
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
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
    Password = masaStackConfig.RedisModel.RedisPassword,
    ClientName = builder.Configuration.GetValue<string>("HOSTNAME")!
};
builder.Services.AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain());

RedisConfigurationOptions redis;
string pmServiceUrl, authServiceUrl;

#if DEBUG
redis = builder.Environment.EnvironmentName == "Development" ? AppSettings.GetModel<RedisConfigurationOptions>("LocalRedisOptions") : redisOption;
builder.Services.AddDaprStarter(opt =>
{
    opt.AppId = appid;
    opt.DaprHttpPort = 3606;
    opt.DaprGrpcPort = 3607;
});
#else
redis = redisOption;
#endif

pmServiceUrl = masaStackConfig.GetPmServiceDomain();
authServiceUrl = masaStackConfig.GetAuthServiceDomain();
builder.Services.AddMasaIdentity(options =>
{
    options.Environment = IdentityClaimConsts.ENVIRONMENT;
    options.UserName = IdentityClaimConsts.USER_NAME;
    options.UserId = IdentityClaimConsts.USER_ID;
    options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
}).AddAuthenticationCore()
    .AddAuthClient(authServiceUrl, redisOption)
    .AddPmClient(pmServiceUrl)
    .AddDccClient(redisOption)
    .AddMultilevelCache(envAppid,
        distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(redis),
        multilevelCacheOptions =>
        {
            multilevelCacheOptions.SubscribeKeyPrefix = MasaStackProject.TSC.Name;
            multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey;
        });
builder.Services.AddMapster();
builder.Services.AddI18n(Path.Combine("Resources", "I18n"));
builder.Services.AddStackMiddleware().AddHealthChecks();

var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
#if DEBUG
    .AddEndpointsApiExplorer()
    //.AddSwaggerGen()
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
#endif
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
        .UseIntegrationEventBus(options =>
        {
            options.UseDapr()
            .UseEventBus();
        })
        .UseUoW<TscDbContext>(dbOptions =>
        {
            var constr = masaStackConfig.GetConnectionString(MasaStackProject.TSC.Name);
            if (isPgsql)
            {
                var assembly = typeof(Masa.Tsc.EFCore.PostgreSQL.TscDbPgContextFactory).Assembly;
                TscDbContext.RegistAssembly(assembly);
                dbOptions.UsePgsql(constr, options => options.MigrationsAssembly("Masa.Tsc.EFCore.PostgreSQL")).UseFilter();
            }
            else
            {
                if (constr.IndexOf("TrustServerCertificate=true", StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    if (constr.EndsWith(';'))
                        constr = $"{constr}TrustServerCertificate=true;";
                    else
                        constr = $"{constr};TrustServerCertificate=true;";
                }
                TscDbContext.RegistAssembly(typeof(Masa.Tsc.EFCore.Sqlserver.TscDbSqlserverContextFactory).Assembly);
                dbOptions.UseSqlServer(constr, options => options.MigrationsAssembly("Masa.Tsc.EFCore.Sqlserver")).UseFilter();
            }
        })
        .UseRepository<TscDbContext>();
    })
    .AddServices(builder, new[] { typeof(IDirectoryRepository).Assembly, typeof(Masa.Tsc.Service.Admin.Services.TraceService).Assembly });

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#endif
app.UseRouting();
app.UseCors();

//app.UseMASAHttpReponseLog();
app.UseAuthentication();
app.UseAuthorization();

app.UseI18n();
app.UseIsolation();
app.UseStackMiddleware();
if (isPgsql)
    await builder.Services.MigratePgAsync();
else
    await builder.Services.MigrateSqlAsync();
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
            context.ToResult(userStatusException.Message, 293);
        }
    };
});

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
});
app.UseHttpsRedirection();
app.UseHealthChecks("/healthy");

app.Run();