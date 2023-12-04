// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Const;

internal class ConfigConst
{
    public const string ConfigRoot = "Appsettings";

    private static AppSettingConfiguration Configuration;

    private readonly static int[] DefaultErrorStatus = new int[] { 500 };

    public static void SetConfiguration(AppSettingConfiguration config)
    {
        Configuration = config;
    }

    public static string LogIndex => Configuration?.LogIndex ?? "masa-stack-logs-0.6.1";

    public static string TraceIndex => Configuration?.TraceIndex ?? "masa-stack-traces-0.6.1";

    public static int[] TraceErrorStatus => Configuration?.Trace?.ErrorStatus ?? DefaultErrorStatus;

    public static bool IsElasticsearch => Configuration.IsElasticsearch;

    public static bool IsClickhouse => Configuration.IsClickHouse;

    public static string ClikhouseConnection => Configuration.Clickhouse?.Connection!;

    public static string ClickHouseLogSourceTable => Configuration.Clickhouse?.LogSource!;

    public static string ClickHouseTaceSourceTable => Configuration.Clickhouse?.TraceSource!;

    internal sealed class StorageConst
    {
        public static bool IsElasticSearch { get; private set; }

        public static bool IsClickhouse { get; private set; }

        public static bool HasInit => IsClickhouse || IsElasticSearch;

        public static void Reset()
        {
            IsClickhouse = false;
            IsElasticSearch = false;
        }

        public static void SetElasticSearch()
        {
            if (!IsClickhouse && !IsElasticSearch)
                IsElasticSearch = true;
        }

        public static void SetClickhouse()
        {
            if (!IsClickhouse && !IsElasticSearch)
                IsClickhouse = true;
        }

        private StorageConst() { }
    }
}
