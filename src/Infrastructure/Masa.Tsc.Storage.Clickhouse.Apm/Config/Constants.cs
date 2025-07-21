// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Config;

internal static class Constants
{
    public static string ErrorTable { get; private set; }

    public static string ExceptErrorTable { get; private set; }

    private const string AggregateTable = "otel_trace_metrics@interval";

    public static string AggregateRootTable { get; private set; }

    public static string DurationTable { get; private set; }

    public static string DurationCountTable { get; private set; }

    public static string ModelsTable { get; private set; }

    public static readonly int[] DefaultErrorStatus = new int[] { 400, 500, 501, 502, 503, 504, 505 };

    public static readonly string[] INTERVALS = new string[] { "1 minute", "30 minute", "1 hour", "1 day", "1 week", "1 month" };

    private static string InitAggregateTable(string interval, string suffix)
    {
        if (string.IsNullOrEmpty(interval))
            interval = INTERVALS[0];
        return AggregateTable.Replace("@interval", interval.Replace(" ", $"_")) + "_" + suffix;
    }

    public static string GetAggregateTable(string interval)
    {
        return DicAggregateTable.TryGetValue(interval, out var table) ? table : default!;
    }

    public static readonly Dictionary<string, string> DicAggregateTable = new();

    public static void Init(string database, string suffix)
    {
        if (!string.IsNullOrEmpty(database))
            database = $"{database}.";
        ErrorTable = $"{database}otel_errors_{suffix}";
        DurationTable = $"{database}otel_traces_spans_duration_{suffix}";
        DurationCountTable = $"{database}otel_traces_spans_duration_count_{suffix}";
        ModelsTable = $"{database}tsc_phone_models_{suffix}";
        ExceptErrorTable = $"{database}tsc_except_errors_{suffix}";
        AggregateRootTable = $"{database}otel_trace_metrics_masa";
        foreach (var key in INTERVALS)
        {
            DicAggregateTable.Add(key, database + InitAggregateTable(key, suffix));
        }
        MasaStackClickhouseConnection.SetEnableExceptError(ExceptErrorTable);
    }
}
