// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Extensions;

public static class ClickhouseInit
{
    /// <summary>后端 Trace 写入：1.5.1（旧 Span 键）、1.9.0（semconv，与历史 InitTraceView1_9_0 一致）。</summary>
    public static readonly string[] BackendTraceSdkVersions =
    [
        OpenTelemetrySdks.OpenTelemetrySdk1_5_1,
        OpenTelemetrySdks.OpenTelemetrySdk1_9_0
    ];

    /// <summary>APM 前端 Trace 写入：lonsid、1.9.0（同 lonsid）、webjs 1.25.1。</summary>
    public static readonly string[] FrontendTraceSdkVersions =
    [
        OpenTelemetrySdks.OpenTelemetrySdk1_5_1_Lonsid,
        OpenTelemetrySdks.OpenTelemetrySdk1_9_0,
        OpenTelemetrySdks.OpenTelemetryJSSdk1_25_1
    ];

    internal static ILogger Logger { get; set; }

    internal static MasaStackClickhouseConnection Connection { get; private set; }

    public static void Init(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logfactory = serviceProvider.GetService<ILoggerFactory>();
        Logger = logfactory?.CreateLogger("Masa.Contrib.StackSdks.Tsc.Clickhouse")!;
        try
        {
            Connection = serviceProvider.GetRequiredService<MasaStackClickhouseConnection>();
            if (!ExistsTable(Connection, MasaStackClickhouseConnection.TraceSourceTable))
                throw new ArgumentNullException(MasaStackClickhouseConnection.TraceSourceTable);
            if (!ExistsTable(Connection, MasaStackClickhouseConnection.LogSourceTable))
                throw new ArgumentNullException(MasaStackClickhouseConnection.LogSourceTable);
            InitLog(MasaStackClickhouseConnection.LogTable, MasaStackClickhouseConnection.LogTable.Replace(".", ".v_"), MasaStackClickhouseConnection.LogSourceTable);
            InitTrace(MasaStackClickhouseConnection.TraceHttpServerTable, MasaStackClickhouseConnection.TraceSourceTable, MasaStackClickhouseConnection.TraceHttpServerTable.Replace(".", ".v_"), "where SpanKind in ('SPAN_KIND_SERVER','Server') and mapContainsKeyLike(SpanAttributes, 'http.%')", BackendTraceSdkVersions, TraceMaterializedViewKind.BackendApi);
            InitTrace(MasaStackClickhouseConnection.TraceHttpClientTable, MasaStackClickhouseConnection.TraceSourceTable, MasaStackClickhouseConnection.TraceHttpClientTable.Replace(".", ".v_"), "where SpanKind in ('SPAN_KIND_CLIENT','Client') and mapContainsKeyLike(SpanAttributes, 'http.%')", BackendTraceSdkVersions, TraceMaterializedViewKind.BackendApi);
            InitTrace(MasaStackClickhouseConnection.TraceOtherClientTable, MasaStackClickhouseConnection.TraceSourceTable, MasaStackClickhouseConnection.TraceOtherClientTable.Replace(".", ".v_"), "where not (SpanKind in ('SPAN_KIND_SERVER','Server') and mapContainsKeyLike(SpanAttributes, 'http.%')) and not (SpanKind in('SPAN_KIND_CLIENT','Client') and mapContainsKeyLike(SpanAttributes, 'http.%'))", BackendTraceSdkVersions, TraceMaterializedViewKind.BackendApi);
            InitMappingTable();
            var timezoneStr = GetTimezone(Connection);
            MasaStackClickhouseConnection.TimeZone = TZConvert.GetTimeZoneInfo(timezoneStr);
        }
        finally
        {
            Connection?.Dispose();
        }
    }

    public static void InitLog(string table, string viewTable, string sourceTable)
    {
        string sql = @$"CREATE TABLE {MasaStackClickhouseConnection.LogTable}
(
    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `TraceId` String CODEC(ZSTD(1)),
    `SpanId` String CODEC(ZSTD(1)),
    `TraceFlags` UInt32 CODEC(ZSTD(1)),
    `SeverityText` LowCardinality(String) CODEC(ZSTD(1)),
    `SeverityNumber` Int32 CODEC(ZSTD(1)),
    `ServiceName` LowCardinality(String) CODEC(ZSTD(1)),
    `Body` String CODEC(ZSTD(1)),
    `ResourceSchemaUrl` String CODEC(ZSTD(1)),
    `Resources` String CODEC(ZSTD(1)),
    `ScopeSchemaUrl` String CODEC(ZSTD(1)),
    `ScopeName` String CODEC(ZSTD(1)),
    `ScopeVersion` String CODEC(ZSTD(1)),
    `Scopes` String CODEC(ZSTD(1)),
    `Logs` String CODEC(ZSTD(1)),
	
	`Resource.service.namespace` String CODEC(ZSTD(1)),	
	`Resource.service.version` String CODEC(ZSTD(1)),	
	`Resource.service.instance.id` String CODEC(ZSTD(1)),	
	
	`Attributes.TaskId`  String CODEC(ZSTD(1)),
    `Attributes.exception.type`  String CODEC(ZSTD(1)),
	`Attributes.exception.message`  String CODEC(ZSTD(1)),   
    `Attributes.http.target`  String CODEC(ZSTD(1)),
    `Attributes.userid`  String CODEC(ZSTD(1)),
    
    ResourceAttributesKeys Array(String) CODEC(ZSTD(1)),
    ResourceAttributesValues Array(String) CODEC(ZSTD(1)),
    LogAttributesKeys Array(String) CODEC(ZSTD(1)),
    LogAttributesValues Array(String) CODEC(ZSTD(1)),
    `LogAttributes` JSON CODEC(ZSTD(1)),
    `ResourceAttributes` JSON CODEC(ZSTD(1)),

    INDEX idx_log_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_span_id SpanId TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_serviceinstanceid `Resource.service.instance.id` TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_severitytext SeverityText TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_taskid `Attributes.TaskId` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_exceptiontype `Attributes.exception.type` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_userid `Attributes.userid` TYPE bloom_filter(0.001) GRANULARITY 1,

	INDEX idx_string_body Body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_exceptionmessage Attributes.exception.message TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1    
)
ENGINE = MergeTree
//PARTITION BY toDate(Timestamp)
ORDER BY (
 Timestamp, 
 ServiceName,
 TraceId,
 Attributes.exception.type, 
 Body,
 Attributes.TaskId,
 Attributes.http.target,
 Attributes.userid,
 SeverityText,
 `Resource.service.namespace`,
 Attributes.exception.message,
 SpanId
 )
TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};
";
        InitTable(table, sql);
        InitLogView(viewTable, sourceTable);
    }

    public static void InitLogView(string name, string sourceName)
    {
        var sql = $@"CREATE MATERIALIZED VIEW {name} TO {MasaStackClickhouseConnection.LogTable}
AS
SELECT
Timestamp,
if((LogAttributes['traceid']) != '', LogAttributes['traceid'], TraceId) AS TraceId,
if((LogAttributes['spanid']) != '', LogAttributes['spanid'], SpanId) AS SpanId,
TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,ResourceSchemaUrl,toJSONString(ResourceAttributes) as Resources,
ScopeSchemaUrl,ScopeName,ScopeVersion,toJSONString(ScopeAttributes) as Scopes,toJSONString(LogAttributes) as Logs,
ResourceAttributes['service.namespace'] as `Resource.service.namespace`,ResourceAttributes['service.version'] as `Resource.service.version`,
ResourceAttributes['service.instance.id'] as `Resource.service.instance.id`,
LogAttributes['TaskId'] as `Attributes.TaskId`,
LogAttributes['exception.type'] as `Attributes.exception.type`,
LogAttributes['exception.message'] as `Attributes.exception.message`,
LogAttributes['RequestPath'] as `Attributes.http.target`,
LogAttributes['userid'] as `Attributes.userid`,
mapKeys(ResourceAttributes) as ResourceAttributesKeys,mapValues(ResourceAttributes) as ResourceAttributesValues,
mapKeys(LogAttributes) as LogAttributesKeys,mapValues(LogAttributes) as LogAttributesValues,
cast((mapKeys(LogAttributes), mapValues(LogAttributes)), 'Map(String, String)') as LogAttributes,
cast((mapKeys(ResourceAttributes), mapValues(ResourceAttributes)), 'Map(String, String)') as ResourceAttributes
FROM {sourceName}
";

        InitTable(name, sql);
    }

    private static void InitTraceTable(string table)
    {
        string sql =
            @$"CREATE TABLE {table}
(
    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `TraceId` String CODEC(ZSTD(1)),
    `SpanId` String CODEC(ZSTD(1)),
    `ParentSpanId` String CODEC(ZSTD(1)),
    `TraceState` String CODEC(ZSTD(1)),
    `SpanName` LowCardinality(String) CODEC(ZSTD(1)),
    `SpanKind` LowCardinality(String) CODEC(ZSTD(1)),
    `ServiceName` LowCardinality(String) CODEC(ZSTD(1)),
    `Resources` String CODEC(ZSTD(1)),
    `ScopeName` String CODEC(ZSTD(1)),
    `ScopeVersion` String CODEC(ZSTD(1)),
    `Spans` String CODEC(ZSTD(1)),
    `Duration` Int64 CODEC(ZSTD(1)),
    `StatusCode` LowCardinality(String) CODEC(ZSTD(1)),
    `StatusMessage` String CODEC(ZSTD(1)),
    `Events.Timestamp` Array(DateTime64(9)) CODEC(ZSTD(1)),
    `Events.Name` Array(LowCardinality(String)) CODEC(ZSTD(1)),
    `Events.Attributes` Array(Map(LowCardinality(String), String)) CODEC(ZSTD(1)),
    `Links.TraceId` Array(String) CODEC(ZSTD(1)),
    `Links.SpanId` Array(String) CODEC(ZSTD(1)),
    `Links.TraceState` Array(String) CODEC(ZSTD(1)),
    `Links.Attributes` Array(Map(LowCardinality(String), String)) CODEC(ZSTD(1)),

	`Resource.service.namespace` String CODEC(ZSTD(1)),	
	`Resource.service.version` String CODEC(ZSTD(1)),	
	`Resource.service.instance.id` String CODEC(ZSTD(1)),

	`Attributes.http.status_code` String CODEC(ZSTD(1)),
	`Attributes.http.response_content_body` String CODEC(ZSTD(1)),
	`Attributes.http.request_content_body` String CODEC(ZSTD(1)),
	`Attributes.http.target` String CODEC(ZSTD(1)),
    `Attributes.http.url` String CODEC(ZSTD(1)),
    `Attributes.http.method` String CODEC(ZSTD(1)),    
    `Attributes.enduser.id` String CODEC( ZSTD(1)),
    `Attributes.masa.ui.traceid` String CODEC(ZSTD(1)),
    `Attributes.exception.type` String CODEC(ZSTD(1)),
	`Attributes.exception.message` String CODEC(ZSTD(1)),    

    `ResourceAttributesKeys` Array(String) CODEC(ZSTD(1)),
    `ResourceAttributesValues` Array(String) CODEC(ZSTD(1)),
    `SpanAttributesKeys` Array(String) CODEC(ZSTD(1)),
    `SpanAttributesValues` Array(String) CODEC(ZSTD(1)),
    `SpanAttributes` JSON CODEC(ZSTD(1)),
    `ResourceAttributes` JSON CODEC(ZSTD(1)),

    INDEX idx_trace_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_span_id SpanId TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_servicenamespace Resource.service.namespace TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_serviceinstanceid Resource.service.instance.id TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_statuscode Attributes.http.status_code TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_trace_exceptiontype Attributes.exception.type TYPE bloom_filter(0.001) GRANULARITY 1,	
    INDEX idx_string_target Attributes.http.target TYPE bloom_filter(0.001) GRANULARITY 1, 
    INDEX idx_string_method Attributes.http.method TYPE bloom_filter(0.001) GRANULARITY 1,	
    INDEX idx_string_userid Attributes.enduser.id TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_string_url Attributes.http.url TYPE bloom_filter(0.001) GRANULARITY 1,	    
    INDEX idx_string_masa_ui_traceId Attributes.masa.ui.traceid TYPE bloom_filter(0.001) GRANULARITY 1,
	
	INDEX idx_string_requestbody Attributes.http.request_content_body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_responsebody Attributes.http.response_content_body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_exceptionmessage Attributes.exception.message TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1
)
ENGINE = MergeTree
//PARTITION BY toDate(Timestamp)
ORDER BY (
 Timestamp, 
 ServiceName,
 TraceId, 
 Attributes.http.target,
 Attributes.http.url,
 Attributes.exception.type, 
 Attributes.http.status_code,
 Attributes.http.method,
 `Attributes.enduser.id`,
 Attributes.masa.ui.traceid,
 Attributes.exception.message, 
 Resource.service.namespace,
 SpanKind,
 SpanId
 )
TTL toDateTime(Timestamp) + toIntervalDay({MasaStackClickhouseConnection.TTL_Days})
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};
";
        InitTable(table, sql);
    }

    public static void InitTrace(string table, string sourceTable, string viewTable, string where, string[] versions, TraceMaterializedViewKind viewKind = TraceMaterializedViewKind.BackendApi)
    {
        InitTraceTable(table);
        InitTraceViewUnified(viewTable, table, sourceTable, where, versions, viewKind);
    }

    /// <summary>单条物化视图：按 TraceMaterializedViewKind 合并各 SDK 的 WHERE 与字段分支。</summary>
    private static void InitTraceViewUnified(string viewTable, string table, string sourceTable, string where, string[] versions, TraceMaterializedViewKind viewKind)
    {
        var versionFilter = BuildTraceSdkVersionWhere(versions, viewKind);
        if (versionFilter is null)
            return;

        var httpFieldSelect = viewKind switch
        {
            TraceMaterializedViewKind.BackendApi => BuildBackendTraceHttpFieldSelect(),
            TraceMaterializedViewKind.ApmFrontend => BuildApmFrontendTraceHttpFieldSelect(),
            _ => throw new ArgumentOutOfRangeException(nameof(viewKind), viewKind, null)
        };

        var sql = $@"CREATE MATERIALIZED VIEW {viewTable} TO {table}
AS
SELECT
    Timestamp,TraceId,SpanId,ParentSpanId,TraceState,SpanName,SpanKind,ServiceName,toJSONString(ResourceAttributes) AS Resources,
    ScopeName,ScopeVersion,toJSONString(SpanAttributes) AS Spans,
    Duration,StatusCode,StatusMessage,Events.Timestamp,Events.Name,Events.Attributes,
    Links.TraceId,Links.SpanId,Links.TraceState,Links.Attributes,

    ResourceAttributes['service.namespace'] AS `Resource.service.namespace`,
    ResourceAttributes['service.version'] AS `Resource.service.version`,
    ResourceAttributes['service.instance.id'] AS `Resource.service.instance.id`,

{httpFieldSelect}
    SpanAttributes['enduser.id'] AS `Attributes.enduser.id`,
    SpanAttributes['masa.ui.traceid'] AS `Attributes.masa.ui.traceid`,
    SpanAttributes['exception.type'] AS `Attributes.exception.type`,
    SpanAttributes['exception.message'] AS `Attributes.exception.message`,

    mapKeys(ResourceAttributes) AS ResourceAttributesKeys,
    mapValues(ResourceAttributes) AS ResourceAttributesValues,
    mapKeys(SpanAttributes) AS SpanAttributesKeys,
    mapValues(SpanAttributes) AS SpanAttributesValues,
    cast((mapKeys(SpanAttributes), mapValues(SpanAttributes)), 'Map(String, String)') AS SpanAttributes,
    cast((mapKeys(ResourceAttributes), mapValues(ResourceAttributes)), 'Map(String, String)') AS ResourceAttributes
FROM {sourceTable}
{where}{versionFilter}
";
        InitTable(viewTable, sql);
    }

    /// <summary>后端：1.5.1 使用旧键；1.9.0 使用原 semconv（http.response.status_code、http.route/url.path、concat url、http.request.method）。</summary>
    private static string BuildBackendTraceHttpFieldSelect() =>
        $@"    multiIf(
        ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}',
            SpanAttributes['http.status_code'],
        SpanAttributes['http.response.status_code']) AS `Attributes.http.status_code`,
    SpanAttributes['http.response_content_body'] AS `Attributes.http.response_content_body`,
    SpanAttributes['http.request_content_body'] AS `Attributes.http.request_content_body`,
    multiIf(
        ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}',
            SpanAttributes['http.target'],
        if(mapContains(SpanAttributes,'http.route'),SpanAttributes['http.route'],SpanAttributes['url.path'])) AS `Attributes.http.target`,
    multiIf(
        ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}',
            SpanAttributes['http.url'],
        concat(SpanAttributes['url.path'],SpanAttributes['url.query'])) AS `Attributes.http.url`,
    multiIf(
        ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}',
            SpanAttributes['http.method'],
        SpanAttributes['http.request.method']) AS `Attributes.http.method`,";

    /// <summary>前端：lonsid/1.9.0 用 http.status_code；webjs 1.25.1 用 http.response.status_code；其余 HTTP 列与历史一致。</summary>
    private static string BuildApmFrontendTraceHttpFieldSelect() =>
        $@"    multiIf(
        ResourceAttributes['telemetry.sdk.version'] IN ('{OpenTelemetrySdks.OpenTelemetrySdk1_5_1_Lonsid}','{OpenTelemetrySdks.OpenTelemetrySdk1_9_0}'),
            SpanAttributes['http.status_code'],
        SpanAttributes['http.response.status_code']) AS `Attributes.http.status_code`,
    SpanAttributes['http.response_content_body'] AS `Attributes.http.response_content_body`,
    SpanAttributes['http.request_content_body'] AS `Attributes.http.request_content_body`,
    SpanAttributes['http.target'] AS `Attributes.http.target`,
    SpanAttributes['http.url'] AS `Attributes.http.url`,
    SpanAttributes['http.method'] AS `Attributes.http.method`,";

    private static string? BuildTraceSdkVersionWhere(string[] versions, TraceMaterializedViewKind viewKind)
    {
        var set = new HashSet<string>(versions ?? Array.Empty<string>());
        var parts = new List<string>(4);
        if (viewKind == TraceMaterializedViewKind.BackendApi)
        {
            if (set.Contains(OpenTelemetrySdks.OpenTelemetrySdk1_5_1))
                parts.Add($"ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1}'");
            if (set.Contains(OpenTelemetrySdks.OpenTelemetrySdk1_9_0))
                parts.Add($"ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_9_0}'");
        }
        else
        {
            if (set.Contains(OpenTelemetrySdks.OpenTelemetrySdk1_5_1_Lonsid))
                parts.Add($"ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_5_1_Lonsid}'");
            if (set.Contains(OpenTelemetrySdks.OpenTelemetrySdk1_9_0))
                parts.Add($"ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetrySdk1_9_0}'");
            if (set.Contains(OpenTelemetrySdks.OpenTelemetryJSSdk1_25_1))
                parts.Add($"(ResourceAttributes['telemetry.sdk.language'] = 'webjs' AND ResourceAttributes['telemetry.sdk.version'] = '{OpenTelemetrySdks.OpenTelemetryJSSdk1_25_1}')");
        }

        return parts.Count == 0 ? null : $" and ({string.Join(" or ", parts)})";
    }

    private static void InitMappingTable()
    {
        var mappingTable = "otel_mapping_";
        var sql = new string[]{
$@"
CREATE TABLE {MasaStackClickhouseConnection.MappingTable}
(
    Id UInt64(64),
    `Name` String CODEC(ZSTD(1)),
    `Type` String CODEC(ZSTD(1))
)
ENGINE = ReplacingMergeTree(Id)
PRIMARY KEY (`Id`)
ORDER BY (`Id`,`Type`,`Name`)
SETTINGS index_granularity = 8192
{MasaStackClickhouseConnection.StorgePolicy};",
@$"CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.MappingTable.Replace(mappingTable,"v_otel_traces_attribute_mapping")} to {MasaStackClickhouseConnection.MappingTable}
as
select 
 DISTINCT sipHash64(concat(Names,'trace_attributes')) as Id, Names as `Name`,'trace_attributes' AS `Type` 
 from
(
SELECT arraySort(mapKeys(SpanAttributes)) AS Names    
FROM {MasaStackClickhouseConnection.TraceSourceTable}
) t
Array join Names",
$@"CREATE MATERIALIZED VIEW  {MasaStackClickhouseConnection.MappingTable.Replace(mappingTable,"v_otel_traces_resource_mapping")} to {MasaStackClickhouseConnection.MappingTable}
as
select 
 DISTINCT sipHash64(concat(Names,'trace_resource')) as Id, Names as `Name`,'trace_resource' AS `Type` 
 from
(
SELECT arraySort(mapKeys(ResourceAttributes)) AS Names    
FROM {MasaStackClickhouseConnection.TraceSourceTable}
) t
Array join Names",
$@"CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.MappingTable.Replace(mappingTable,"v_otel_logs_attribute_mapping")} to {MasaStackClickhouseConnection.MappingTable}
as
select 
 DISTINCT sipHash64(concat(Names,'log_attributes')) as Id, Names as `Name`,'log_attributes' AS `Type` 
 from
(
SELECT arraySort(mapKeys(LogAttributes)) AS Names    
FROM {MasaStackClickhouseConnection.LogSourceTable}
) t
Array join Names",
$@"CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.MappingTable.Replace(mappingTable,"v_otel_logs_resource_mapping")} to {MasaStackClickhouseConnection.MappingTable}
as
select 
 DISTINCT sipHash64(concat(Names,'log_resource')) as Id, Names as `Name`,'log_resource' AS `Type` 
 from
(
SELECT arraySort(mapKeys(ResourceAttributes)) AS Names    
FROM {MasaStackClickhouseConnection.LogSourceTable}
) t
Array join Names",
$@"insert into {MasaStackClickhouseConnection.MappingTable}
values 
(sipHash64('Timestamplog_basic'),'Timestamp','log_basic'),
(sipHash64('TraceIdplog_basic'),'TraceId','log_basic'),
(sipHash64('SpanIdlog_basic'),'SpanId','log_basic'),
(sipHash64('TraceFlaglog_basic'),'TraceFlag','log_basic'),
(sipHash64('SeverityTextlog_basic'),'SeverityText','log_basic'),
(sipHash64('SeverityNumberlog_basic'),'SeverityNumber','log_basic'),
(sipHash64('Bodylog_basic'),'Body','log_basic'),

(sipHash64('Timestamptrace_basic'),'Timestamp','trace_basic'),
(sipHash64('TraceIdtrace_basic'),'TraceId','trace_basic'),
(sipHash64('SpanIdtrace_basic'),'SpanId','trace_basic'),
(sipHash64('ParentSpanIdtrace_basic'),'ParentSpanId','trace_basic'),
(sipHash64('TraceStatetrace_basic'),'TraceState','trace_basic'),
(sipHash64('SpanKindtrace_basic'),'SpanKind','trace_basic'),
(sipHash64('Durationtrace_basic'),'Duration','trace_basic');
" };
        InitTable(MasaStackClickhouseConnection.MappingTable, sql);
    }

    private static void InitTable(string tableName, params string[] sqls)
    {
        var database = Connection.ConnectionSettings.Database!;
        if (!string.IsNullOrEmpty(database))
            tableName = tableName.Substring(database.Length + 1);

        if (Convert.ToInt32(Connection.ExecuteScalar($"select count() from system.tables where database ='{database}' and name in ['{tableName}']")) > 0)
            return;
        if (sqls == null || sqls.Length == 0)
            return;
        foreach (var sql in sqls)
        {
            ExecuteSql(Connection, sql);
        }
    }

    internal static bool ExistsTable(MasaStackClickhouseConnection connection, string tableName)
    {
        var database = connection.ConnectionSettings.Database!;
        if (!string.IsNullOrEmpty(database))
            tableName = tableName.Substring(database.Length + 1);
        return Convert.ToInt32(connection.ExecuteScalar($"select count() from system.tables where database ='{database}' and name in ['{tableName}']")) > 0;
    }

    public static void InitTable(MasaStackClickhouseConnection connection, string tableName, params string[] sqls)
    {
        if (ExistsTable(connection, tableName))
            return;
        if (sqls == null || sqls.Length == 0)
            return;
        foreach (var sql in sqls)
        {
            ExecuteSql(connection, sql);
        }
    }

    internal static void ExecuteSql(this IDbConnection connection, string sql)
    {
        using var cmd = connection.CreateCommand();
        if (connection.State != ConnectionState.Open)
            connection.Open();
        cmd.CommandText = sql;
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Init table sql error:{RawSql}", sql);
        }
    }

    private static string GetTimezone(MasaStackClickhouseConnection connection)
    {
        using var cmd = connection.CreateCommand();
        if (connection.State != ConnectionState.Open)
            connection.Open();
        var sql = "select timezone()";
        cmd.CommandText = sql;
        try
        {
            return cmd.ExecuteScalar()?.ToString()!;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "ExecuteSql {RawSql} error", sql);
            throw;
        }
    }
}