// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Identity.IdentityModel;
using Masa.Contrib.BasicAbility.Tsc;
using Masa.Stack.Components;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddMasaStackComponentsForServer("wwwroot/i18n", builder.Configuration["AuthServiceBaseAddress"]);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);


//string otlpUri = builder.Configuration.GetSection("otlpUri").Value;
//var resources = ResourceBuilder.CreateDefault();
//resources.AddMasaService(builder.Configuration.GetSection("masa:tsc").Get<MasaObservableOptions>());

////metrics
//builder.Services.AddMasaMetrics(builder =>
//{
//    builder.SetResourceBuilder(resources);
//    builder.AddOtlpExporter(option =>
//    {
//        option.Endpoint = new Uri(otlpUri);
//    });
//});

////trcaing
//builder.Services.AddMasaTracing(configure =>
//{
//    configure.AspNetCoreInstrumentationOptions.AppendBlazorFilter(configure);
//    configure.BuildTraceCallback = builder =>
//    {
//        builder.SetResourceBuilder(resources);
//        builder.AddOtlpExporter(option =>
//        {
//            option.Endpoint = new Uri(otlpUri);
//        });
//    };
//});

////logging
//builder.Logging.AddOpenTelemetry(builder =>
//{
//    builder.SetResourceBuilder(resources);
//    builder.IncludeScopes = true;
//    builder.IncludeFormattedMessage = true;
//    builder.ParseStateValues = true;
//    builder.AddOtlpExporter(option =>
//    {
//        option.Endpoint = new Uri(otlpUri);
//    });
//});

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