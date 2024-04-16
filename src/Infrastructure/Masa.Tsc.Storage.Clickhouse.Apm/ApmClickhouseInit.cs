// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal static class ApmClickhouseInit
{
    public static void Init(MasaStackClickhouseConnection connection)
    {
        InitErrorTable(connection);
        InitAggregateTable(connection);
    }

    private static void InitErrorTable(MasaStackClickhouseConnection connection)
    {
        var sql = new string[]{
        @$"CREATE TABLE {Constants.ErrorTable}
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
",
$@"CREATE MATERIALIZED VIEW {Constants.ErrorTable.Replace(".",".v_")} TO {Constants.ErrorTable}
AS
SELECT
    Timestamp,TraceId,SpanId, if(position(Body, '\n') > 0,extract(Body, '[^\n\r]+'),Body) `Attributes.exception.message`,LogAttributes['exception.type'] AS `Attributes.exception.type`,
    ServiceName,ResourceAttributes['service.namespace'] AS `Resource.service.namespace`, LogAttributes['RequestPath'] AS `Attributes.http.target`,
multiIf(
  length(`Attributes.exception.message`)-150<=0,`Attributes.exception.message`,  
  extract(`Attributes.exception.message`, '[^,:\\.£º£¬\{{\[]+')) AS MsgGroupKey
FROM {MasaStackClickhouseConnection.LogSourceTable}
WHERE mapContains(LogAttributes, 'exception.type')
"};
        ClickhouseInit.InitTable(connection, Constants.ErrorTable, sql);
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
$@"CREATE MATERIALIZED VIEW {tableName.Replace(".",".v_")} TO {tableName}
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
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    toStartOfInterval(Timestamp,INTERVAL {interval}) AS Timestamp,
    avgState(Duration) AS Latency,
    countState(1) AS Throughput,
    sumState(has(['400','500','501','502','503'],`Attributes.http.status_code`)) as Failed,
    quantileState(0.99)(Duration) as P99,
    quantileState(0.95)(Duration) as P95 
FROM {MasaStackClickhouseConnection.TraceTable}
WHERE
SpanKind='SPAN_KIND_SERVER'
GROUP BY
    ServiceName,
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    Timestamp"
        };
        ClickhouseInit.InitTable(connection, tableName, sql);
    }
}
