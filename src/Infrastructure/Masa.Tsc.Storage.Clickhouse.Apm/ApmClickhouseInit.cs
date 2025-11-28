// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal static class ApmClickhouseInit
{
    private static string AppLogTable = default!;
    private static string AppTraceTable = default!;

    public static void Init(MasaStackClickhouseConnection connection, string suffix, string? logTable = null, string? traceTable = null)
    {
        InitAppTable(connection, suffix, logTable, traceTable);
        InitModelTable(connection);
        InitErrorTable(connection);
        InitAggregateTable(connection);
        InitDurationTable(connection);
        InitDurationCountTable(connection);
        //InitExceptErrorTable(connection);
    }

    private static void InitErrorTable(MasaStackClickhouseConnection connection)
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
//PARTITION BY toDate(Timestamp)
ORDER BY (Timestamp,
 ServiceName, 
 `Attributes.exception.type`,
 `MsgGroupKey`,
`Attributes.http.target`,
`Attributes.exception.message`,
`Resource.service.namespace`
)
TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};
";
        ClickhouseInit.InitTable(connection, Constants.ErrorTable, sql);
        InitErrorView(connection, Constants.ErrorTable.Replace(".", ".v_"), MasaStackClickhouseConnection.LogSourceTable);
        if (!string.IsNullOrEmpty(AppLogTable))
        {
            InitErrorView(connection, Constants.ErrorTable.Replace(".", ".v_app_"), AppLogTable);
        }
    }

    private static void InitErrorView(MasaStackClickhouseConnection connection, string table, string source)
    {
        var sql =
$@"CREATE MATERIALIZED VIEW {table} TO {Constants.ErrorTable}
AS
SELECT
    Timestamp,TraceId,SpanId, 
   multiIf(position(LogAttributes['exception.stacktrace'],'\n')==0,substring(LogAttributes['exception.stacktrace'],position(LogAttributes['exception.stacktrace'],': ')+2),
position(LogAttributes['exception.stacktrace'],'\n')-position(LogAttributes['exception.stacktrace'],': ')-2<=0,substring(LogAttributes['exception.stacktrace'],position(LogAttributes['exception.stacktrace'],'\n')+1,position(LogAttributes['exception.stacktrace'],'\n',position(LogAttributes['exception.stacktrace'],'\n')+1)-position(LogAttributes['exception.stacktrace'],'\n')-2),
substring(LogAttributes['exception.stacktrace'],
position(LogAttributes['exception.stacktrace'],': ')+2,
position(LogAttributes['exception.stacktrace'],'\n')-position(LogAttributes['exception.stacktrace'],': ')-2)) AS `Attributes.exception.message`,
    LogAttributes['exception.type'] AS `Attributes.exception.type`,
    ServiceName,ResourceAttributes['service.namespace'] AS `Resource.service.namespace`, LogAttributes['RequestPath'] AS `Attributes.http.target`,
`Attributes.exception.message` AS MsgGroupKey
FROM {source}
WHERE SeverityText in ['Error','Critical'] and mapContains(LogAttributes, 'exception.type')
";
        ClickhouseInit.InitTable(connection, table, sql);
    }

    private static void InitAppTable(MasaStackClickhouseConnection connection, string suffix, string? logTable, string? traceTable)
    {
        if (!string.IsNullOrEmpty(logTable))
        {
            AppLogTable = logTable;
            ClickhouseInit.InitLog(MasaStackClickhouseConnection.LogTable, MasaStackClickhouseConnection.LogTable.Replace(".", ".v_app_"), logTable);
        }
        if (!string.IsNullOrEmpty(traceTable))
        {
            AppTraceTable = traceTable;
            ClickhouseInit.InitTrace(MasaStackClickhouseConnection.TraceHttpServerTable, traceTable, MasaStackClickhouseConnection.TraceHttpServerTable.Replace(".", ".v_app_"), "where SpanKind in ('SPAN_KIND_SERVER','Server') and mapContainsKeyLike(SpanAttributes, 'http.%')");
            ClickhouseInit.InitTrace(MasaStackClickhouseConnection.TraceHttpClientTable, traceTable, MasaStackClickhouseConnection.TraceHttpClientTable.Replace(".", ".v_app_"), "where SpanKind in ('SPAN_KIND_CLIENT','Client') and mapContainsKeyLike(SpanAttributes, 'http.%')");
            ClickhouseInit.InitTrace(MasaStackClickhouseConnection.TraceOtherClientTable, traceTable, MasaStackClickhouseConnection.TraceOtherClientTable.Replace(".", ".v_app_"), "where not (SpanKind in ('SPAN_KIND_SERVER','Server') and mapContainsKeyLike(SpanAttributes, 'http.%')) and not (SpanKind in ('SPAN_KIND_CLIENT','Client') and mapContainsKeyLike(SpanAttributes, 'http.%'))");
        }
    }

    private static void InitAggregateTable(MasaStackClickhouseConnection connection)
    {
        InitAggregateRootTable(connection);
        foreach (var item in Constants.DicAggregateTable)
        {
            InitAggregateTable(connection, item.Key, item.Value);
        }
    }

    private static void InitAggregateRootTable(MasaStackClickhouseConnection connection)
    {
        var sql = $@"CREATE TABLE {Constants.AggregateRootTable}
(
   `dimensions` String,
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
PARTITION BY (dimensions,toYYYYMM(Timestamp))
ORDER BY (
dimensions,
Timestamp,
 ServiceName,
 Attributes.http.target,
Attributes.http.method,
 Resource.service.namespace 
 )
 TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};";
        ClickhouseInit.InitTable(connection, Constants.AggregateRootTable, sql);
    }

    private static void InitAggregateTable(MasaStackClickhouseConnection connection, string interval, string tableName)
    {
        var viewTable = tableName.Replace(".", ".v_");
        InitAggregateTable1_5_1(connection, interval, tableName, viewTable, MasaStackClickhouseConnection.TraceSourceTable);
        if (!string.IsNullOrEmpty(AppTraceTable))
        {
            InitAggregateTable1_5_1(connection, interval, tableName, tableName.Replace(".", ".v_app_"), AppTraceTable);
            InitAggregateTable1_25_1(connection, interval, tableName, tableName.Replace(".", ".v_app_"), AppTraceTable);
        }
        InitAggregateTable1_9_0(connection, interval, tableName, viewTable, MasaStackClickhouseConnection.TraceSourceTable);
    }

    private static void InitAggregateTable1_5_1(MasaStackClickhouseConnection connection, string interval, string tableName, string viewTable, string sourceTable)
    {
        viewTable = $"{viewTable}_{OpenTelemetrySdks.OpenTelemetrySdk1_5_1.Replace('.', '_')}";
        var sql =
$@"CREATE MATERIALIZED VIEW {viewTable} TO {Constants.AggregateRootTable}
(
   `dimensions` String,
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
     '{interval.Replace(' ', '_')}' AS dimensions,
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
FROM {sourceTable}
WHERE
SpanKind in ('SPAN_KIND_SERVER','Server')
and ResourceAttributes ['telemetry.sdk.version'] in ['{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}','{OpenTelemetrySdks.OpenTelemetrySdk1_5_1_Lonsid}']
GROUP BY
    ServiceName,
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    Timestamp";
        ClickhouseInit.InitTable(connection, viewTable, sql);
    }

    private static void InitAggregateTable1_25_1(MasaStackClickhouseConnection connection, string interval, string tableName, string viewTable, string sourceTable)
    {
        viewTable = $"{viewTable}_{OpenTelemetrySdks.OpenTelemetryJSSdk1_25_1.Replace('.', '_')}";
        var sql =
$@"CREATE MATERIALIZED VIEW {viewTable} TO {Constants.AggregateRootTable}
(
    `dimensions` String,
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
     '{interval.Replace(' ', '_')}' AS dimensions,
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
FROM {sourceTable}
WHERE
SpanKind in ('SPAN_KIND_SERVER','Server') and `Attributes.http.target`!=''
and ResourceAttributes ['telemetry.sdk.version'] in ['{OpenTelemetrySdks.OpenTelemetryJSSdk1_25_1}']
GROUP BY
    ServiceName,
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    Timestamp";
        ClickhouseInit.InitTable(connection, viewTable, sql);
    }

    private static void InitAggregateTable1_9_0(MasaStackClickhouseConnection connection, string interval, string tableName, string viewTable, string sourceTable)
    {
        viewTable = $"{viewTable}_{OpenTelemetrySdks.OpenTelemetrySdk1_9_0.Replace('.', '_')}";
        var sql =
$@"CREATE MATERIALIZED VIEW {viewTable} TO {Constants.AggregateRootTable}
(
    `dimensions` String,
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
     '{interval.Replace(' ', '_')}' AS dimensions,
    ServiceName,
    ResourceAttributes['service.namespace'] `Resource.service.namespace`,
    if(mapContains(SpanAttributes,'http.route'),SpanAttributes['http.route'],SpanAttributes['url.path']) `Attributes.http.target`,
    SpanAttributes['http.request.method'] `Attributes.http.method`,
    toStartOfInterval(Timestamp,INTERVAL {interval}) AS Timestamp,
    avgState(Duration) AS Latency,
    countState(1) AS Throughput,
    sumState(has(['400','500','501','502','503'],SpanAttributes['http.response.status_code'])) as Failed,
    quantileState(0.99)(Duration) as P99,
    quantileState(0.95)(Duration) as P95 
FROM {sourceTable}
WHERE
SpanKind in ('SPAN_KIND_SERVER','Server')
and ResourceAttributes ['telemetry.sdk.version'] in ['{OpenTelemetrySdks.OpenTelemetrySdk1_9_0}']
GROUP BY
    ServiceName,
    `Resource.service.namespace`,
    `Attributes.http.target`,
    `Attributes.http.method`,
    Timestamp";
        ClickhouseInit.InitTable(connection, viewTable, sql);
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
Order by (`Brand`,`Model`,`CodeAlis`)
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};";
        ClickhouseInit.InitTable(connection, Constants.ModelsTable, sql);
    }

    private static void InitDurationTable(MasaStackClickhouseConnection connection)
    {
        var table = Constants.DurationTable;
        var sql = $@"create table {Constants.DurationTable}(
Timestamp DateTime64(9) CODEC(Delta(8), ZSTD(1)),
ServiceName String CODEC(ZSTD(1)),
`Resource.service.namespace` String CODEC(ZSTD(1)),
`Attributes.http.method` String CODEC(ZSTD(1)),
`Attributes.http.status_code` String CODEC(ZSTD(1)),
`Attributes.http.target` String CODEC(ZSTD(1)),
TraceId String CODEC(ZSTD(1)),
Duration Int64 CODEC(ZSTD(1)),

INDEX idx_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
INDEX idx_namespace `Resource.service.namespace` TYPE bloom_filter(0.001) GRANULARITY 1,
INDEX idx_method `Attributes.http.method` TYPE bloom_filter(0.001) GRANULARITY 1,
INDEX idx_statuscode `Attributes.http.status_code` TYPE bloom_filter(0.001) GRANULARITY 1,
INDEX idx_url `Attributes.http.target` TYPE bloom_filter(0.001) GRANULARITY 1,
INDEX idx_traceid TraceId TYPE bloom_filter(0.001) GRANULARITY 1
)
ENGINE = MergeTree
ORDER BY (
 Timestamp,
 ServiceName,
 TraceId,
 Attributes.http.target, 
 Attributes.http.status_code,
 Attributes.http.method,
 Resource.service.namespace,
Duration)
TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};";
        var viewTableName = table.Replace(".", ".v_");
        var sqlView =
 $@"CREATE MATERIALIZED VIEW {viewTableName} TO {table}
AS
select Timestamp,ServiceName,Resource.service.namespace,Attributes.http.method,Attributes.http.status_code,Attributes.http.target,TraceId,Duration
from 
{MasaStackClickhouseConnection.TraceHttpServerTable}
where `Attributes.http.target`!=''
";
        ClickhouseInit.InitTable(connection, table, sql);
        ClickhouseInit.InitTable(connection, viewTableName, sqlView);
    }

    private static void InitDurationCountTable(MasaStackClickhouseConnection connection)
    {
        var table = Constants.DurationCountTable;
        var sql = $@"create table {Constants.DurationCountTable}(
                            Timestamp DateTime64(9) CODEC(Delta(8), ZSTD(1)),
                            ServiceName String CODEC(ZSTD(1)),
                            `Resource.service.namespace` String CODEC(ZSTD(1)),
                            `Attributes.http.method` String CODEC(ZSTD(1)),
                            `Attributes.http.target` String CODEC(ZSTD(1)),
                            Duration Int64 CODEC(ZSTD(1)),
                            Total AggregateFunction(count,UInt8),
                            
                            INDEX idx_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
                            INDEX idx_namespace `Resource.service.namespace` TYPE bloom_filter(0.001) GRANULARITY 1,
                            INDEX idx_method `Attributes.http.method` TYPE bloom_filter(0.001) GRANULARITY 1,                           
                            INDEX idx_url `Attributes.http.target` TYPE bloom_filter(0.001) GRANULARITY 1
                            )
                            ENGINE = AggregatingMergeTree
                            //PARTITION BY toYYYYMM(Timestamp)
                            ORDER BY (
                             Timestamp,
                             ServiceName,
                             Attributes.http.target, 
                             Attributes.http.method,
                             Resource.service.namespace,
                             Duration)
                            TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
                            SETTINGS index_granularity = 8192
                            {MasaStackClickhouseConnection.StorgePolicy};";
        var viewTableName = table.Replace(".", ".v_");
        var sqlView =
 $@"CREATE MATERIALIZED VIEW {viewTableName} TO {table}
        AS
        SELECT
            toStartOfInterval(Timestamp,toIntervalMinute(1)) AS Timestamp,
            ServiceName,
            `Resource.service.namespace`,    
            `Attributes.http.method`, 
            `Attributes.http.target`,
            floor(Duration/1000000) as Duration,
            countState(1) AS Total
        FROM {Constants.DurationTable}
        where `Attributes.http.target`!=''
        GROUP BY
            ServiceName,
            `Resource.service.namespace`,
            `Attributes.http.target`,
            `Attributes.http.method`,
            Timestamp,
            Duration";
        ClickhouseInit.InitTable(connection, table, sql);
        ClickhouseInit.InitTable(connection, viewTableName, sqlView);
    }

    private static void InitExceptErrorTable(MasaStackClickhouseConnection connection)
    {
        var table = Constants.ExceptErrorTable;
        var sql = $@"CREATE TABLE {table}(
    `Id` String CODEC(ZSTD(1)),
    `Environment` String CODEC(ZSTD(1)),
    `Project` String CODEC(ZSTD(1)),
    `Service` String CODEC(ZSTD(1)),
    `Type` String CODEC(ZSTD(1)),
    `Message` String CODEC(ZSTD(1)),
    `Comment` String CODEC(ZSTD(1)),
    `CreationTime` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `Creator` String CODEC(ZSTD(1)),
    `ModificationTime` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `Modifier` String CODEC(ZSTD(1)),
    `IsDeleted` Bool,

    INDEX idx_error_environment Environment TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_project Project TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_service Service TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_type Type TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_error_message Message TYPE bloom_filter(0.001) GRANULARITY 1
)
ENGINE = ReplacingMergeTree(ModificationTime)
ORDER BY (Environment,
 Project,
 Service,
 `Type`,
 `Message`)
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};";
        ClickhouseInit.InitTable(connection, table, sql);
    }
}
