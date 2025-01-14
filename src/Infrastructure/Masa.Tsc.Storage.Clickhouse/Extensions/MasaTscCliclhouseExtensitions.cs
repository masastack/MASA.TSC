// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MasaTscCliclhouseExtensitions
{
    internal static ILogger? Logger { get; }

    public static IServiceCollection AddMASAStackClickhouse(this IServiceCollection services,
        string connectionStr,
        string suffix = "masastack",
        string? logSourceTable = null,
        string? traceSourceTable = null,
        string? storagePolicy = null,
        int ttlDays = 30,
        Action<IDbConnection>? configer = null)
    {
        _ = new ClickhouseStorageConst();
        if (!string.IsNullOrEmpty(storagePolicy))
            MasaStackClickhouseConnection.StorgePolicy = $",storage_policy = '{storagePolicy}'";
        if (ttlDays > 0)
            MasaStackClickhouseConnection.TTL_Days = ttlDays;
        services.AddScoped(services => new MasaStackClickhouseConnection(connectionStr, suffix, logSourceTable, traceSourceTable))
            .AddScoped<ILogService, LogService>()
            .AddScoped<ITraceService, TraceService>();
        ClickhouseInit.Init(services);
        configer?.Invoke(services.BuildServiceProvider().GetRequiredService<MasaStackClickhouseConnection>()!);
        return services;
    }
}
