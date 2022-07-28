// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Data.Contracts.EF;
using Masa.Tsc.Service.Admin;

var builder = WebApplication.CreateBuilder(args);

var elasearchUrls = builder.Configuration.GetSection("Masa:Elastic:nodes").Get<string[]>();
builder.Configuration.ConfigureElasticIndex();
builder.Services.AddCaller(option =>
{
    option.UseHttpClient(builder =>
    {
        builder.Name = ElasticConst.ES_HTTP_CLIENT_NAME;
        builder.Configure = opt =>
        {
            opt.BaseAddress = new Uri(elasearchUrls[0]);
        };
    });
});
builder.Services.AddElasticsearchClient("tsclog", elasearchUrls);
builder.AddObservable();
builder.Configuration.ConfigureElasticIndex();

builder.Services.AddDaprClient();
builder.Services.AddPrometheusClient(builder.Configuration.GetSection("Masa:Prometheus").Value);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "";
    options.RequireHttpsMetadata = false;
    options.Audience = "";
});

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddAuthClient(builder.Configuration["AuthServiceBaseAddress"]);
builder.Services.AddPmClient(builder.Configuration["Masa:PmUrl"]);

var app = builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
                new string[] {}
            }
        });
    })
     .AddIntegrationEventBus<IntegrationEventLogService>(options =>
     {
         options.UseDapr();
         options.UseUoW<TscDbContext>(dbOptions => dbOptions.UseSqlServer().UseFilter())
                .UseEventLog<TscDbContext>()
                .UseEventBus()
                .UseRepository<TscDbContext>();
     })
    .AddServices(builder);


app.MigrateDbContext<TscDbContext>((context, services) =>
{
    //var logger = services.GetRequiredService<ILogger<TscDbContext>>();
    //new AuthDbContextSeed().SeedAsync(context, logger).Wait();
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
