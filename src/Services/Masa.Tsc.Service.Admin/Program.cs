﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

//using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
//using MASA.BuildingBlocks.Dispatcher.Events;
//using MASA.Utils.Data.Elasticsearch;

using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Provider;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
using Masa.Contrib.Data.UoW.EFCore;

var builder = WebApplication.CreateBuilder(args);
//builder.AddMasaConfiguration(configurationBuilder =>
//{
//    configurationBuilder.UseDcc();
//});

var elasearchUrls = builder.Configuration.GetSection("Masa:Elastic:nodes").Get<string[]>();
builder.Configuration.ConfigureElasticIndex();

builder.Services.AddCaller(option =>
{
    option.UseHttpClient(ElasticConst.ES_HTTP_CLIENT_NAME, builder =>
     {
         builder.BaseAddress = elasearchUrls[0];
     });
});
builder.Services.AddElasticsearchClient("tsclog", elasearchUrls);
builder.AddObservable();

builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc(builder.Configuration.GetSection("Masa:Dcc").Get<DccOptions>(), default, default);
});
//#if DEBUG
//builder.Services.AddDaprStarter(opt =>
//{
//    opt.DaprHttpPort = 3600;
//    opt.DaprGrpcPort = 3601;
//});
//#endif
builder.Services.AddDaprClient();
builder.Services.AddPrometheusClient(builder.GetMasaConfiguration().Local.GetSection("Masa:Prometheus").Value);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = builder.GetMasaConfiguration().Local.GetValue<string>("IdentityServerUrl");
    options.RequireHttpsMetadata = false;
    //options.Audience = "";
    options.TokenValidationParameters.ValidateAudience = false;
    options.MapInboundClaims = false;
});

builder.Services.AddMasaIdentityModel(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddAuthClient(builder.GetMasaConfiguration().Local["Masa:Auth:ServiceBaseAddress"]);
builder.Services.AddPmClient(builder.GetMasaConfiguration().Local["Masa:Pm:ServiceBaseAddress"]);

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
    .AddIntegrationEventBus<IntegrationEventLogService>(options =>
    {
        options.UseDapr()
        .UseUoW<TscDbContext>(dbOptions => dbOptions.UseSqlServer().UseFilter())
        .UseEventLog<TscDbContext>()
        .UseEventBus()
        .UseRepository<TscDbContext>();
    })
    .AddServices(builder);

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
