// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl;

public static class ObservableExtensions
{
    public static void AddObservable(this WebApplicationBuilder builder)
    {
        string otlpUrl = builder.Configuration.GetSection("Masa:Observable:OtlpUrl").Value;
        var resources = ResourceBuilder.CreateDefault();
        resources.AddMasaService(builder.Configuration.GetSection("Masa:Observable").Get<MasaObservableOptions>());
        var url = new Uri(otlpUrl);

        //metrics
        builder.Services.AddMasaMetrics(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(option =>
            {
                option.Endpoint = url;
            });
        });

        //trcaing
        builder.Services.AddMasaTracing(configure =>
        {
            configure.AspNetCoreInstrumentationOptions.AppendBlazorFilter(configure);
            configure.BuildTraceCallback = builder =>
            {
                builder.SetResourceBuilder(resources);
                builder.AddOtlpExporter(option =>
                {
                    option.Endpoint = url;
                });
            };
        });

        //logging
        builder.Logging.AddOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.IncludeScopes = true;
            builder.IncludeFormattedMessage = true;
            builder.ParseStateValues = true;
            builder.AddOtlpExporter(option =>
            {
                option.Endpoint = url;
            });
        });

        builder.Services.AddTscApiCaller(builder.Configuration["Masa:Tsc:ServiceBaseAddress"]);
    }
}
