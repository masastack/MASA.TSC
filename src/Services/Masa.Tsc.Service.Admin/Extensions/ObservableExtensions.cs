// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Extenision;

public static class ObservableExtensions
{
    public static void AddObservable(this WebApplicationBuilder builder)
    {
        var option = builder.Configuration.GetSection("masa:tsc").Get<MasaObservableOptions>();
        var resources = ResourceBuilder.CreateDefault().AddMasaService(option);
        var opltUrl = builder.Configuration.GetSection("masa:otlpUrl").Get<string>();
        var uri = new Uri(opltUrl);

        builder.Services.AddMasaMetrics(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options => options.Endpoint = uri);
        });

        builder.Services.AddMasaTracing(builder =>
        {
            builder.AspNetCoreInstrumentationOptions.AppendDefaultFilter(builder);

            builder.BuildTraceCallback = options =>
            {
                options.SetResourceBuilder(resources);
                options.AddOtlpExporter(opt => opt.Endpoint = uri);
            };
        });

        builder.Logging.AddMasaOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options => options.Endpoint = uri);
        });
    }
}
