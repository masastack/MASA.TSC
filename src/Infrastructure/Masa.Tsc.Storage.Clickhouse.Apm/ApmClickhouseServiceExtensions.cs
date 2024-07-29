// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ApmClickhouseServiceExtensions
{
    internal static ILogger Logger { get; }

    public static IServiceCollection AddMASAStackApmClickhouse(this IServiceCollection services, string connectionStr, string suffix = "masastack",
        string? logSourceTable = null, string? traceSourceTable = null,
        string? appLogSourceTable = null, string? AppTraceSourceTable = null)
    {
        services.AddMASAStackClickhouse(connectionStr, suffix, logSourceTable, traceSourceTable, con =>
         {
             var clickhouseConnection = (MasaStackClickhouseConnection)con;
             Constants.Init(clickhouseConnection.ConnectionSettings.Database, suffix);
             ApmClickhouseInit.Init(clickhouseConnection, suffix, appLogSourceTable, AppTraceSourceTable);
         });
        services.AddScoped<IApmService, ClickhouseApmServiceNew>();
        return services;
    }
}
