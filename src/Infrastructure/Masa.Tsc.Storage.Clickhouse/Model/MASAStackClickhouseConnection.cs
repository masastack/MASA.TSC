// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
namespace Masa.Tsc.Storage.Clickhouse;

public sealed class MasaStackClickhouseConnection : ClickHouseConnection
{
    public static string LogSourceTable { get; private set; }

    public static string TraceSourceTable { get; private set; }

    public static string LogTable { get; private set; }

    public static string TraceTable { get; private set; }

    public static string TraceSpanTable { get; private set; }

    public static string TraceClientTable { get; private set; }

    public static string MappingTable { get; private set; }

    public static TimeZoneInfo TimeZone { get; set; }

    public static DateTime ToTimeZone(DateTime time)
    {
        var newTime = time.Kind == DateTimeKind.Unspecified ? time : DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
        return new DateTimeOffset(newTime + TimeZone.BaseUtcOffset, TimeZone.BaseUtcOffset).DateTime;
    }

    public object LockObj { get; init; } = new();

    public MasaStackClickhouseConnection(string connection, string suffix, string? logSourceTable = null, string? traceSourceTable = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(suffix);
        ConnectionString = connection;
        logSourceTable ??= "otel_logs";
        traceSourceTable ??= "otel_traces";

        string database = string.IsNullOrEmpty(ConnectionSettings.Database) ? default! : $"{ConnectionSettings.Database}.";

        LogTable = $"{database}{logSourceTable}_{suffix}";
        TraceTable = $"{database}{traceSourceTable}_{suffix}";
        TraceSourceTable = $"{database}{traceSourceTable}";
        LogSourceTable = $"{database}{logSourceTable}";
        MappingTable = $"{database}otel_mapping_{suffix}";
        TraceSpanTable = $"{database}{traceSourceTable}_spans_{suffix}";
        TraceClientTable = $"{database}{traceSourceTable}_clients_{suffix}";
    }
}
