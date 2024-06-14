// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal static class ApmClickhouseInit
{
    public static void Init(MasaStackClickhouseConnection connection, string suffix, string? logTable = null, string? traceTable = null)
    {
        InitModelTable(connection);
        InitErrorTable(connection, suffix, logTable);
        InitAppTable(connection, suffix, logTable, traceTable);
        InitAggregateTable(connection);
    }

    private static void InitErrorTable(MasaStackClickhouseConnection connection, string? suffix = null, string? appLogTable = null)
    {
        var sql = @$"CREATE TABLE {Constants.ErrorTable}
(
    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `TraceId` String CODEC(ZSTD(1)),
    `SpanId` String CODEC(ZSTD(1)),
    `Attributes.exception.message` String CODEC(ZSTD(1)),
    `Attributes.exception.type` String CODEC(ZSTD(1)),
    `ServiceName` String CODEC(ZSTD(1)),
    `Resource.service.namespace` String CODEC(ZSTD(1)),
    `Attributes.http.target` String CODEC(ZSTD(1)),
    `MsgGroupKey` String CODEC(ZSTD(1)),

    INDEX idx_error_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_spanid SpanId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_environment `Resource.service.namespace` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_type `Attributes.exception.type` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_endpoint `Attributes.http.target` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_message `Attributes.exception.message` TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
    INDEX idx_error_msggroupkey MsgGroupKey TYPE bloom_filter(0.001) GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toDate(Timestamp)
ORDER BY (Timestamp,
 ServiceName,
 `Resource.service.namespace`,
 `Attributes.exception.type`,
 `MsgGroupKey`,
`Attributes.http.target`)
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;
";
        ClickhouseInit.InitTable(connection, Constants.ErrorTable, sql);
        InitErrorView(connection, Constants.ErrorTable.Replace(".", ".v_"), MasaStackClickhouseConnection.LogSourceTable);
        if (!string.IsNullOrEmpty(appLogTable))
        {
            string database = string.IsNullOrEmpty(connection.ConnectionSettings.Database) ? default! : $"{connection.ConnectionSettings.Database}.";
            InitErrorView(connection, $"{database}v_{appLogTable}_errors", appLogTable);
        }
    }

    private static void InitErrorView(MasaStackClickhouseConnection connection, string table, string source)
    {
        var sql =
$@"CREATE MATERIALIZED VIEW {table} TO {Constants.ErrorTable}
AS
SELECT
    Timestamp,TraceId,SpanId, if(position(Body, '\n') > 0,extract(Body, '[^\n\r]+'),Body) `Attributes.exception.message`,LogAttributes['exception.type'] AS `Attributes.exception.type`,
    ServiceName,ResourceAttributes['service.namespace'] AS `Resource.service.namespace`, LogAttributes['RequestPath'] AS `Attributes.http.target`,
multiIf(
  length(`Attributes.exception.message`)-150<=0,`Attributes.exception.message`,  
  extract(`Attributes.exception.message`, '[^,:\\.£º£¬\{{\[]+')) AS MsgGroupKey
FROM {source}
WHERE mapContains(LogAttributes, 'exception.type')
";
        ClickhouseInit.InitTable(connection, table, sql);
    }

    private static void InitAppTable(MasaStackClickhouseConnection connection, string suffix, string? logTable, string? traceTable)
    {
        string database = string.IsNullOrEmpty(connection.ConnectionSettings.Database) ? default! : $"{connection.ConnectionSettings.Database}.";

        if (!string.IsNullOrEmpty(logTable))
        {
            ClickhouseInit.InitLogView($"{database}v_{logTable}_{suffix}", logTable);
        }
        if (!string.IsNullOrEmpty(traceTable))
        {
            ClickhouseInit.InitTraceView($"{database}v_{traceTable}_spans_{suffix}", MasaStackClickhouseConnection.TraceSpanTable, traceTable, "where SpanKind =='SPAN_KIND_SERVER'");
            ClickhouseInit.InitTraceView($"{database}v_{traceTable}_clients_{suffix}", MasaStackClickhouseConnection.TraceClientTable, traceTable, "where SpanKind =='SPAN_KIND_CLIENT'");
        }
    }

    private static void InitAggregateTable(MasaStackClickhouseConnection connection)
    {
        foreach (var item in Constants.DicAggregateTable)
        {
            InitAggregateTable(connection, item.Key, item.Value);
        }
    }

    private static void InitAggregateTable(MasaStackClickhouseConnection connection, string interval, string tableName)
    {
        var viewTable = tableName.Replace(".", ".v_");
        var sql = new string[] {
        $@"CREATE TABLE {tableName}
(
    `ServiceName` String,
    `Resource.service.namespace` String,
    `Attributes.http.target` String,
    `Attributes.http.method` String,
    `Timestamp` DateTime64(9),
    `Latency` AggregateFunction(avg,Int64),
    `Throughput` AggregateFunction(count,UInt8),
    `Failed` AggregateFunction(sum,UInt8),
    `P99` AggregateFunction(quantile(0.99),Int64),
    `P95` AggregateFunction(quantile(0.95), Int64)
)
ENGINE = AggregatingMergeTree()
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (
 ServiceName,
 Attributes.http.target,
 Resource.service.namespace,
 Timestamp
 )
 TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192",
$@"CREATE MATERIALIZED VIEW {viewTable} TO {tableName}
(
    `ServiceName` String,
    `Resource.service.namespace` String,
     `Attributes.http.target` String,
    `Attributes.http.method` String,
    `Timestamp` DateTime64(9),
    `Latency` AggregateFunction(avg, Float64),
    `Throughput` AggregateFunction(count, UInt8),
    `Failed` AggregateFunction(count, UInt8),
    `P99` AggregateFunction(quantile(0.99),Int64),
    `P95` AggregateFunction(quantile(0.95), Int64)
) AS
SELECT
    ServiceName,
    ResourceAttributes['service.namespace'] `Resource.service.namespace`,
    SpanAttributes['http.target'] `Attributes.http.target`,
    SpanAttributes['http.method'] `Attributes.http.method`,
    toStartOfInterval(Timestamp,INTERVAL {interval}) AS Timestamp,
    avgState(Duration) AS Latency,
    countState(1) AS Throughput,
    sumState(has(['400','500','501','502','503'],SpanAttributes['http.status_code'])) as Failed,
    quantileState(0.99)(Duration) as P99,
    quantileState(0.95)(Duration) as P95 
FROM {MasaStackClickhouseConnection.TraceSourceTable}
WHERE
SpanKind='SPAN_KIND_SERVER'
GROUP BY
    ServiceName,
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    Timestamp"
        };

        ClickhouseInit.InitTable(connection, tableName, sql[0]);
        ClickhouseInit.InitTable(connection, viewTable, sql[1]);
    }

    private static void InitModelTable(MasaStackClickhouseConnection connection)
    {
        var sql = $@"create table {Constants.ModelsTable}(
`Model` String CODEC(ZSTD(1)),
`Dtype` String CODEC(ZSTD(1)),
`Brand` String CODEC(ZSTD(1)),
`BrandName` String CODEC(ZSTD(1)),
`Code` String CODEC(ZSTD(1)),
`CodeAlis` String CODEC(ZSTD(1)),
`ModeName` String CODEC(ZSTD(1)),
`VerName` String CODEC(ZSTD(1))
)
engine = MergeTree
Order by (`Brand`,`Model`)
SETTINGS index_granularity = 8192";
        ClickhouseInit.InitTable(connection, Constants.ModelsTable, sql);
    }
}
