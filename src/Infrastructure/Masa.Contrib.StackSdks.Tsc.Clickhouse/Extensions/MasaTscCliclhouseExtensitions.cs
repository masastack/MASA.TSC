// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Tsc.Clickhouse.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class MasaTscCliclhouseExtensitions
{
    internal static ILogger? Logger { get; private set; }

    public static IServiceCollection AddMASAStackClickhouse(this IServiceCollection services, string connectionStr, string suffix = "masastack", string? logSourceTable = null, string? traceSourceTable = null, Action<IDbConnection>? configer = null)
    {
        services.AddScoped(services => new MasaStackClickhouseConnection(connectionStr, suffix, logSourceTable, traceSourceTable))
            .AddScoped<ILogService, LogService>()
            .AddScoped<ITraceService, TraceService>();
        ClickhouseInit.Init(services);
        configer?.Invoke(services.BuildServiceProvider().GetRequiredService<MasaStackClickhouseConnection>()!);
        return services;
    }
}
