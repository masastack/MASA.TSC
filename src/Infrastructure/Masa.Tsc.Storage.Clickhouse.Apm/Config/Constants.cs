// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Config;

internal static class Constants
{
    public static string ErrorTable { get; private set; }

    private const string AggregateTable = "otel_trace_metrics@interval";

    public static string ModelsTable { get; private set; }

    private static string GetAggregateTable(string interval, string suffix)
    {
        if (string.IsNullOrEmpty(interval))
            interval = INTERVALS[0];
        return AggregateTable.Replace("@interval", interval.Replace(" ", $"_")) + "_" + suffix;
    }

    public static string GetAggregateTable(string interval)
    {
        return DicAggregateTable.TryGetValue(interval, out var table) ? table : default!;
    }

    public static readonly int[] DefaultErrorStatus = new int[] { 400, 500, 501, 502, 503, 504, 505 };

    public static readonly string[] INTERVALS = new string[] { "1 minute", "30 minute", "1 hour", "1 day", "1 week", "1 month" };

    public static readonly Dictionary<string, string> DicAggregateTable = new();

    public static void Init(string database, string suffix)
    {
        if (!string.IsNullOrEmpty(database))
            database = $"{database}.";
        ErrorTable = $"{database}otel_errors_{suffix}";
        ModelsTable = $"{database}tsc_phone_models_{suffix}";
        foreach (var key in INTERVALS)
        {
            DicAggregateTable.Add(key, database + GetAggregateTable(key, suffix));
        }
    }
}
