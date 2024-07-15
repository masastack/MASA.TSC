// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Config;

internal static class Constants
{
    public static string ErrorTable { get; private set; }

    private const string AggregateTable = "otel_trace_metrics@interval";
    private const string DurationCountTable = "otel_traces_spans_duration@intervalcount";

    public static string DurationTable { get; private set; }

    public static string DurationCountTable1 { get; private set; }

    public static string ModelsTable { get; private set; }

    private static string GetAggregateTable(string interval, string suffix)
    {
        if (string.IsNullOrEmpty(interval))
            interval = INTERVALS[0];
        return AggregateTable.Replace("@interval", interval.Replace(" ", $"_")) + "_" + suffix;
    }

    private static string GetDurationCountTable(string interval, string suffix)
    {
        if (string.IsNullOrEmpty(interval))
            interval = INTERVALS[0];
        return DurationCountTable.Replace("@interval", interval.Replace(" ", $"_")) + "_" + suffix;
    }

    public static string GetAggregateTable(string interval)
    {
        return DicAggregateTable.TryGetValue(interval, out var table) ? table : default!;
    }

    public static string GetDurationCountTable(string interval)
    {
        return DicDurationCountTable.TryGetValue(interval, out var table) ? table : default!;
    }

    public static readonly int[] DefaultErrorStatus = new int[] { 400, 500, 501, 502, 503, 504, 505 };

    public static readonly string[] INTERVALS = new string[] { "1 minute", "30 minute", "1 hour", "1 day", "1 week", "1 month" };

    public static readonly Dictionary<string, string> DicAggregateTable = new();
    public static readonly Dictionary<string, string> DicDurationCountTable = new();

    public static void Init(string database, string suffix)
    {
        if (!string.IsNullOrEmpty(database))
            database = $"{database}.";
        ErrorTable = $"{database}otel_errors_{suffix}";
        DurationTable = $"{database}otel_traces_spans_duration_{suffix}";
        DurationCountTable1 = $"{database}otel_traces_spans_duration_count_{suffix}";
        ModelsTable = $"{database}tsc_phone_models_{suffix}";
        foreach (var key in INTERVALS)
        {
            DicAggregateTable.Add(key, database + GetAggregateTable(key, suffix));
            DicDurationCountTable.Add(key, database + GetDurationCountTable(key, suffix));
        }
    }
}
