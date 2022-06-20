// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.BasicAbility.Auth;
using Masa.Contrib.BasicAbility.Pm;
using Masa.Contrib.Data.UoW.EF;
using Masa.Tsc.Service.Admin.Extenision;
using Masa.Utils.Caller.HttpClient;
using Masa.Utils.Data.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthClient(builder.Configuration.GetSection("masa:authUri").Value);
builder.Services.AddPmClient(builder.Configuration.GetSection("masa:pmUri").Value);
var elasearchUris = builder.Configuration.GetSection("masa:elastic:nodes").Get<string[]>();
builder.Configuration.ConfigureElasticIndex();
builder.Services.AddCaller(option =>
{
    option.UseHttpClient(builder =>
    {
        builder.Name = ElasticConst.ES_HTTP_CLIENT_NAME;
        builder.Configure = opt =>
        {
            opt.BaseAddress = new Uri(elasearchUris[0]);
        };
    });
});
builder.Services.AddElasticsearchClient("tsclog", elasearchUris);
builder.AddObservable();
builder.Configuration.ConfigureElasticIndex();

builder.Services.AddDaprClient();
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
    //.AddTransient(typeof(IMiddleware<>), typeof(LogMiddleware<>))
    .AddDomainEventBus(builder =>
    {
        builder.UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<TscDbContext>())
        .UseEventBus()
       .UseUoW<TscDbContext>()
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
