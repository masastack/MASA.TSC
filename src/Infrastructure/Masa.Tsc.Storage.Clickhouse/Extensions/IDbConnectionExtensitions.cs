// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse")]
namespace System.Data.Common;

internal static class IDbConnectionExtensitions
{
    public static PaginatedListBase<TraceResponseDto> QueryTrace(this IDbConnection connection, BaseRequestDto query)
    {
        if (query.Conditions != null && query.Conditions.Any(c => c.Name == "MAUI"))
        {
            var list = query.Conditions.ToList();
            list.RemoveAll((c => c.Name == "MAUI"));
            return QueryMAUITrace(connection, query);
        }
        var (where, parameters, ors) = AppendWhere(query);
        var orderBy = AppendOrderBy(query, false);
        var result = new PaginatedListBase<TraceResponseDto>() { Result = new() };
        var start = (query.Page - 1) * query.PageSize;
        if (query.HasPage())
        {
            var countSql = CombineOrs($"select count() as `total` from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors);
            var total = Convert.ToInt64(ExecuteScalar(connection, $"select sum(`total`) from ({countSql})", parameters?.ToArray()));
            result.Total = total;
            if (total <= 0 || start - total >= 0)
                return result;
        }

        var querySql = CombineOrs($"select ServiceName,{ClickhouseHelper.GetName("timestamp", false)},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors, orderBy);
        result.Result = Query(connection, $"select * from {querySql} as t limit {start},{query.PageSize}", parameters?.ToArray(), ConvertTraceDto);

        return result;
    }

    private static PaginatedListBase<TraceResponseDto> QueryMAUITrace(this IDbConnection connection, BaseRequestDto query)
    {
        var (where, parameters, ors) = AppendWhere(query);
        var orderBy = AppendOrderBy(query, false);
        var result = new PaginatedListBase<TraceResponseDto>() { Result = new() };
        var start = (query.Page - 1) * query.PageSize;
        if (query.HasPage())
        {
            var countSql1 = CombineOrs($"select TraceId from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors);
            var countSql2 = CombineOrs($"select TraceId from {MasaStackClickhouseConnection.TraceHttpClientTable} where {where} and ParentSpanId not in(select SpanId from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where})", ors);
            var countSql = $"select count(1) as `total`  from ({countSql1} union all {countSql2})";
            var total = Convert.ToInt64(ExecuteScalar(connection, $"select sum(`total`) from ({countSql})", parameters?.ToArray()));
            result.Total = total;
            if (total <= 0 || start - total >= 0)
                return result;
        }

        var querySql1 = CombineOrs($"select ServiceName,{ClickhouseHelper.GetName("timestamp", false)},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors, orderBy);
        var querySql2 = CombineOrs($"select ServiceName,{ClickhouseHelper.GetName("timestamp", false)},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceHttpClientTable} where {where} and ParentSpanId not in(select SpanId from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where})", ors, orderBy);
        var querySql = $"({querySql1} union all {querySql2})";
        result.Result = Query(connection, $"select * from {querySql} as t {orderBy} limit {start},{query.PageSize}", parameters?.ToArray(), ConvertTraceDto);

        return result;

    }

    private static bool IsExceptError(BaseRequestDto query)
    {
        var list = query.Conditions?.ToList();
        var filter = list?.Find(m => string.Equals("filter", m.Name, StringComparison.OrdinalIgnoreCase));
        if (filter != null)
        {
            list!.Remove(filter);
            query.Conditions = list;
        }

        return filter != null && Convert.ToBoolean(filter.Value.ToString());
    }

    public static PaginatedListBase<LogResponseDto> QueryLog(this IDbConnection connection, BaseRequestDto query)
    {
        bool isExceptError = IsExceptError(query);
        var (where, parameters, ors) = AppendWhere(query, false);
        var orderBy = AppendOrderBy(query, true);
        var start = (query.Page - 1) * query.PageSize;
        var result = new PaginatedListBase<LogResponseDto>() { Result = new() };
        if (query.HasPage())
        {
            var countSql = isExceptError ? $"(select count(1) as `total`  from {CombineOrs($"select Resource.service.namespace,ServiceName,Attributes.exception.type,Attributes.exception.message from {MasaStackClickhouseConnection.LogTable} where {where}",ors)} a left join {MasaStackClickhouseConnection.ExceptErrorTable} b on not IsDeleted and a.`Resource.service.namespace` =b.Environment  and a.ServiceName =b.Service  and a.`Attributes.exception.type` =b.`Type`  and a.`Attributes.exception.message` =b.Message  where b.Service ='')"
                : CombineOrs($"select count(1) as `total` from {MasaStackClickhouseConnection.LogTable} where {where}", ors);
            var total = Convert.ToInt64(ExecuteScalar(connection, $"select sum(`total`) from {countSql}", parameters?.ToArray()));
            result.Total = total;
            if (total <= 0 || start - total >= 0)
                return result;
        }

        var querySql = isExceptError ? $"(select {ClickhouseHelper.GetName("timestamp", true)},TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,Resources,Logs from {CombineOrs($"select {ClickhouseHelper.GetName("timestamp", true)},TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,Resources,Logs,Resource.service.namespace,Attributes.exception.type,Attributes.exception.message from {MasaStackClickhouseConnection.LogTable} where {where} ",ors,orderBy)} a left join {MasaStackClickhouseConnection.ExceptErrorTable} b on not IsDeleted and a.`Resource.service.namespace` =b.Environment  and a.ServiceName =b.Service  and a.`Attributes.exception.type` =b.`Type`  and a.`Attributes.exception.message` =b.Message where b.Service ='' )"
            : CombineOrs($"select {ClickhouseHelper.GetName("timestamp", true)},TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,Resources,Logs from {MasaStackClickhouseConnection.LogTable} where {where}", ors, orderBy);
        result.Result = Query(connection, $"select * from {querySql} as t limit {start},{query.PageSize}", parameters?.ToArray(), ConvertLogDto);

        return result;
    }

    private static string CombineOrs(string sql, IEnumerable<string> ors, string? orderBy = null)
    {
        if (ors == null || !ors.Any())
            return $"({sql} {orderBy})";

        var text = new StringBuilder();
        foreach (var or in ors)
        {
            text.AppendLine($" union all {sql} {or} {orderBy}");
        }
        text.Remove(0, 11).Insert(0, '(').Append(')');
        return text.ToString();
    }

    public static List<MappingResponseDto> GetMapping(this IDbConnection dbConnection, bool isLog)
    {
        var type = isLog ? "log" : "trace";
        var result = dbConnection.Query($"select DISTINCT Name from {MasaStackClickhouseConnection.MappingTable} where `Type`='{type}_basic' order by Name", default, ConvertToMapping);
        if (result == null || result.Count == 0)
            return default!;

        var attributes = dbConnection.Query($"select DISTINCT concat('{ClickhouseHelper.ATTRIBUTE_KEY}',Name)  from {MasaStackClickhouseConnection.MappingTable} where `Type`='{type}_attributes' order by Name", default, ConvertToMapping);
        var resources = dbConnection.Query($"select DISTINCT concat('{ClickhouseHelper.RESOURCE_KEY}',Name)  from {MasaStackClickhouseConnection.MappingTable} where `Type`='{type}_resource' order by Name", default, ConvertToMapping);
        if (attributes != null && attributes.Count > 0) result.AddRange(attributes);
        if (resources != null && resources.Count > 0) result.AddRange(resources);

        return result;
    }

    public static List<TraceResponseDto> GetTraceByTraceId(this IDbConnection connection, BaseRequestDto query)
    {
        //query.Start = default;
        //query.End = default;
        var (where, parameters, _) = AppendWhere(query);
        var timeField = StorageConst.Current.Timestimap;
        return Query(connection,
            $@"select * from (
                    select {timeField},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}
                    union all
                    select {timeField},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceHttpClientTable} where {where}
                    union all
                    select {timeField},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceOtherClientTable} where {where}
                ) as t 
            order by {timeField}
            limit 300", parameters.ToArray(), ConvertTraceDto);
    }

    public static string AppendOrderBy(BaseRequestDto query, bool isLog)
    {
        var field = StorageConst.Current.Timestimap;
        var isDesc = query.Sort?.IsDesc ?? true;
        if (isLog && query.Sort != null && !string.IsNullOrEmpty(query.Sort.Name))
        {
            field = ClickhouseHelper.GetName(query.Sort.Name, isLog);
            isDesc = query.Sort?.IsDesc ?? false;
        }
        return $" order by {field}{(isDesc ? " desc" : "")}";
    }

    public static (string where, List<IDataParameter> @parameters, List<string> ors) AppendWhere(BaseRequestDto query, bool isTrace = true)
    {
        var sql = new StringBuilder();
        var @paramerters = new List<IDataParameter>();

        if (query.Start > DateTime.MinValue && query.Start < DateTime.MaxValue
            && query.End > DateTime.MinValue && query.End < DateTime.MaxValue
            && query.End > query.Start)
        {
            sql.Append($" and {StorageConst.Current.Timestimap} BETWEEN @Start and @End");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "Start", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime2 });
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "End", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime2 });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.Append($" and {ClickhouseHelper.GetName(StorageConst.Current.ServiceName, !isTrace)} = @serviceName");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "serviceName", Value = query.Service });
        }
        if (!string.IsNullOrEmpty(query.Instance))
        {
            sql.Append($" and {ClickhouseHelper.GetName(StorageConst.Current.ServiceInstance, !isTrace)} = @serviceInstanceId");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "serviceInstanceId", Value = query.Instance });
        }
        if (isTrace && !string.IsNullOrEmpty(query.Endpoint))
        {
            sql.Append($" and {ClickhouseHelper.GetName(StorageConst.Current.Trace.URL, !isTrace)} = @httpTarget");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "httpTarget", Value = query.Endpoint });
        }
        if (!string.IsNullOrEmpty(query.TraceId))
        {
            sql.Append($" and {StorageConst.Current.TraceId} = @TraceId");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "TraceId", Value = query.TraceId });
        }

        var ors = AppendKeyword(query.Keyword?.Trim()!, paramerters, isTrace);
        AppendConditions(query.Conditions, paramerters, sql, isTrace);

        if (!string.IsNullOrEmpty(query.RawQuery))
            sql.AppendLine(!query.RawQuery.Trim().StartsWith("and ", StringComparison.CurrentCultureIgnoreCase) ? $" and {query.RawQuery}" : query.RawQuery);

        if (sql.Length > 0)
            sql.Remove(0, 4);
        return (sql.ToString(), @paramerters, ors);
    }

    private static List<string> AppendKeyword(string keyword, List<IDataParameter> @paramerters, bool isTrace = true)
    {
        var sqls = new List<string>();
        if (string.IsNullOrEmpty(keyword))
            return sqls;

        if (keyword.Contains(" and ")
            || keyword.Contains(" or ")
            || keyword.Contains("='")
            || keyword.Contains(" in ")
            || keyword.Contains(" not in ")
            || keyword.Contains(" like ")
            || keyword.Contains(" not like  "))
        {
            sqls.Add(!keyword.StartsWith("and ", StringComparison.CurrentCultureIgnoreCase) ? $" and {keyword}" : keyword);
            return sqls;
        }

        //status_code
        if (isTrace)
        {
            if (int.TryParse(keyword, out var num) && num - -1 > 0 && num - 600 < 0)
            {
                sqls.Add($" and {ClickhouseHelper.GetName(StorageConst.Current.Trace.HttpStatusCode, !isTrace)}=@HttpStatusCode");
                paramerters.Add(new ClickHouseParameter() { ParameterName = "HttpStatusCode", Value = keyword });
                return sqls;
            }
            if (Guid.TryParse(keyword, out var _) && keyword.Length - 32 == 0)
            {
                sqls.Add($" and {StorageConst.Current.TraceId}=@TraceUserId");
                paramerters.Add(new ClickHouseParameter() { ParameterName = "TraceUserId", Value = keyword });
                return sqls;
            }
            else
            {
                sqls.Add(ClickhouseHelper.AppendLike(ClickhouseHelper.GetName(StorageConst.Current.Trace.HttpRequestBody, false), "Keyword", keyword));
                sqls.Add(ClickhouseHelper.AppendLike(ClickhouseHelper.GetName(StorageConst.Current.Trace.URLFull, false), "Keyword", keyword));
            }
        }
        else
        {
            if (keyword.Equals("error", StringComparison.CurrentCultureIgnoreCase))
            {
                sqls.Add($" and {StorageConst.Current.Log.LogLevelText}='Error'");
                return sqls;
            }
            if (keyword.EndsWith("exception", StringComparison.CurrentCultureIgnoreCase) && Regex.IsMatch(keyword, @"[\da-zA-Z]+"))
            {
                sqls.Add($" and {StorageConst.Current.ExceptionType} = '{keyword}'");
                return sqls;
            }

            if (Guid.TryParse(keyword, out var _))
            {
                sqls.Add($" and {StorageConst.Current.TraceId}=@TraceUserId");
                paramerters.Add(new ClickHouseParameter() { ParameterName = "TraceUserId", Value = keyword });
                return sqls;
            }
            sqls.Add(ClickhouseHelper.AppendLike(StorageConst.Current.Log.Body, "Keyword", keyword));
            sqls.Add(ClickhouseHelper.AppendLike(ClickhouseHelper.GetName(StorageConst.Current.ExceptionMessage, !isTrace), "Keyword", keyword));
        }

        paramerters.Add(ClickhouseHelper.GetLikeParameter("Keyword", keyword));
        return sqls;
    }

    private static void AppendConditions(IEnumerable<FieldConditionDto>? conditions, List<IDataParameter> @paramerters, StringBuilder sql, bool isTrace = true)
    {
        if (conditions == null || !conditions.Any())
            return;

        foreach (var item in conditions)
        {
            var name = ClickhouseHelper.GetName(item.Name, !isTrace);

            if (item.Value is DateTime time)
            {
                item.Value = MasaStackClickhouseConnection.ToTimeZone(time);
            }
            else if (string.Equals(name, StorageConst.Current.ServiceName, StringComparison.CurrentCultureIgnoreCase))
            {
                AppendField(item, @paramerters, sql, name, "serviceName");
            }
            else if (string.Equals(name, StorageConst.Current.ServiceInstance, StringComparison.CurrentCultureIgnoreCase))
            {
                AppendField(item, @paramerters, sql, name, "serviceInstanceId");
            }
            else if (string.Equals(name, StorageConst.Current.Environment, StringComparison.CurrentCultureIgnoreCase))
            {
                AppendField(item, @paramerters, sql, name, "serviceNameSpace");
            }
            else if (item.Name.StartsWith(ClickhouseHelper.ATTRIBUTE_KEY, StringComparison.CurrentCultureIgnoreCase))
            {
                var filed = item.Name[ClickhouseHelper.ATTRIBUTE_KEY.Length..];
                AppendField(item, @paramerters, sql, name, filed.Replace('.', '_'));
            }
            else
            {
                AppendField(item, @paramerters, sql, name, name);
            }
        }
    }

    private static void AppendField(FieldConditionDto item, List<IDataParameter> @paramerters, StringBuilder sql, string fieldName, string paramName)
    {
        if (item.Value is string str && string.IsNullOrEmpty(str) || item.Value is IEnumerable<object> collects && !collects.Any())
            return;
        switch (item.Type)
        {
            case ConditionTypes.Equal:
                {
                    if (@paramerters.Exists(p => p.ParameterName == paramName))
                        break;
                    ParseWhere(sql, item.Value, paramerters, fieldName, paramName, "=");
                }
                break;
            case ConditionTypes.NotIn:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"{paramName}s", "not in");
                }
                break;
            case ConditionTypes.In:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"{paramName}s", "in");
                }
                break;
            case ConditionTypes.LessEqual:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"lte_{paramName}", "<=");
                }
                break;
            case ConditionTypes.GreatEqual:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"gte_{paramName}", ">=");
                }
                break;
            case ConditionTypes.Less:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"lt_{paramName}", "<");
                }
                break;
            case ConditionTypes.Great:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"gt_{paramName}", ">");
                }
                break;
            case ConditionTypes.Regex:
                {
                    ParseWhere(sql, $"%{item.Value}%", paramerters, fieldName, $"paramName", "like");
                }
                break;
            case ConditionTypes.NotRegex:
                {
                    ParseWhere(sql, $"%{item.Value}%", paramerters, fieldName, $"paramName", "not like");
                }
                break;
        }
    }

    private static void ParseWhere(StringBuilder sql, object value, List<IDataParameter> @paramerters, string fieldName, string paramName, string compare)
    {
        var dbType = value is DateTime ? DbType.DateTime2 : DbType.AnsiString;
        if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
        {
            var values = Array.CreateInstance(typeof(JsonElement), json.GetArrayLength());
            var position = values.Length - 1;
            do
            {
                values.SetValue(json[position], position--);
            }
            while (position >= 0);
            value = values;
        }

        if (value is IEnumerable)
            sql.Append($" and {fieldName} {compare} (@{paramName})");
        else
            sql.Append($" and {fieldName} {compare} @{paramName}");
        @paramerters.Add(new ClickHouseParameter { ParameterName = $"{paramName}", Value = value, DbType = dbType });
    }

    public static object? ExecuteScalar(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters = null)
    {
        var start = DateTime.Now;
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Length > 0)
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        OpenConnection(dbConnection);
        try
        {
            return cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            MasaTscCliclhouseExtensitions.Logger?.LogError(ex, "execute sql error:{RawSql}, paramters:{Parameters}", sql, parameters);
            throw;
        }
        finally
        {
            var end = DateTime.Now;
            var duration = (end - start).TotalSeconds;
            if (duration - 1 > 0)
                ClickhouseInit.Logger.LogWarning("Clickhouse query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}", duration, sql, parameters);
        }
    }

    private static void OpenConnection(IDbConnection dbConnection)
    {
        if (dbConnection.State == ConnectionState.Closed)
            dbConnection.Open();
    }

    public static List<T> Query<T>(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters, Func<IDataReader, T> parse)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Length > 0)
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        OpenConnection(dbConnection);
        var start = DateTime.Now;
        try
        {
            using var reader = cmd.ExecuteReader();
            if (reader == null)
                return new List<T>();
            var list = new List<T>();
            while (reader.NextResult())
                while (reader.Read())
                {
                    list.Add(parse.Invoke(reader));
                }
            return list;
        }
        catch (Exception ex)
        {
            MasaTscCliclhouseExtensitions.Logger?.LogError(ex, "query sql error:{RawSql}, paramters:{Parameters}", sql, parameters);
            throw;
        }
        finally
        {
            var end = DateTime.Now;
            var duration = (end - start).TotalSeconds;
            if (duration - 1 > 0)
                ClickhouseInit.Logger.LogWarning("Clickhouse query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}", duration, sql, parameters);
        }
    }

    public static MappingResponseDto ConvertToMapping(IDataReader reader)
    {
        return new MappingResponseDto
        {
            Name = reader[0].ToString()!,
            Type = "string"
        };
    }

    public static TraceResponseDto ConvertTraceDto(IDataReader reader)
    {
        var startTime = Convert.ToDateTime(reader[StorageConst.Current.Timestimap]);
        long ns = Convert.ToInt64(reader["Duration"]);
        string resource = reader["Resources"].ToString()!, spans = reader["Spans"].ToString()!;
        var result = new TraceResponseDto
        {
            TraceId = reader["TraceId"].ToString()!,
            EndTimestamp = startTime.AddMilliseconds(ns / 1e6),

            Kind = reader["SpanKind"].ToString()!,
            Name = reader["SpanName"].ToString()!,
            ParentSpanId = reader["ParentSpanId"].ToString()!,
            SpanId = reader["SpanId"].ToString()!,
            Timestamp = startTime
        };
        if (!string.IsNullOrEmpty(resource))
            result.Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(resource)!;
        if (!string.IsNullOrEmpty(spans))
            result.Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(spans)!;
        return result;
    }

    public static LogResponseDto ConvertLogDto(IDataReader reader)
    {
        string resource = reader["Resources"].ToString()!, logs = reader["Logs"].ToString()!;
        var result = new LogResponseDto
        {
            TraceId = reader["TraceId"].ToString()!,
            Body = reader["Body"].ToString()!,
            SeverityNumber = Convert.ToInt32(reader["SeverityNumber"]),
            SeverityText = reader["SeverityText"].ToString()!,
            TraceFlags = Convert.ToInt32(reader["TraceFlags"]),
            SpanId = reader["SpanId"].ToString()!,
            Timestamp = Convert.ToDateTime(reader[StorageConst.Current.Timestimap]),
        };
        if (!string.IsNullOrEmpty(resource))
            result.Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(resource)!;
        if (!string.IsNullOrEmpty(logs))
            result.Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(logs)!;
        return result;
    }

    public static object AggregationQuery(this IDbConnection dbConnection, SimpleAggregateRequestDto requestDto, bool isLog = true)
    {
        var sql = new StringBuilder("select ");
        bool isExceptError = IsExceptError(requestDto);
        var append = new StringBuilder();        
        var name = ClickhouseHelper.GetName(requestDto.Name, isLog);
        AppendAggtype(requestDto, sql, append, name, out var isScalar); 
        var (where, @paremeters, _) = AppendWhere(requestDto, !isLog);       
        if (isLog)
        {
            var table = isExceptError ? $" (select Resource.service.namespace,ServiceName,Attributes.exception.type,Attributes.exception.message,{name} from {MasaStackClickhouseConnection.LogTable} where {where}) a left join {MasaStackClickhouseConnection.ExceptErrorTable} b on not IsDeleted and a.`Resource.service.namespace` =b.Environment  and a.ServiceName =b.Service  and a.`Attributes.exception.type` =b.`Type`  and a.`Attributes.exception.message` =b.Message " : MasaStackClickhouseConnection.LogTable;
            sql.AppendFormat(" from {0}", table);
        }
        else
        {
            sql.AppendFormat(" from {0}", MasaStackClickhouseConnection.TraceHttpServerTable);
        }
        if (isExceptError && isLog)
            sql.Append($" where b.Message=''");
        else
            sql.Append($" where {where}");

        sql.Append(append);
        var paramArray = @paremeters?.ToArray()!;

        if (isScalar)
        {
            return dbConnection.ExecuteScalar(sql.ToString(), paramArray)!;
        }
        else
        {
            return AggTerm(dbConnection, sql.ToString(), paramArray, requestDto.Type, requestDto.AllValue);
        }
    }

    private static object AggTerm(IDbConnection dbConnection, string sql, IDataParameter[] paramArray, AggregateTypes aggregateTypes, bool isAllValue)
    {
        var result = dbConnection.Query(sql, paramArray, reader =>
        {
            if (aggregateTypes == AggregateTypes.GroupBy)
            {
                if (isAllValue)
                    return KeyValuePair.Create(reader[0].ToString(), Convert.ToInt64(reader[1]));
                else
                    return reader[0];
            }
            else
            {
                var time = Convert.ToDateTime(reader[0]);
                var timestamp = new DateTimeOffset(time).ToUnixTimeMilliseconds();
                return KeyValuePair.Create(timestamp, Convert.ToInt64(reader[1]));
            }
        });
        if (aggregateTypes == AggregateTypes.GroupBy)
        {
            if (isAllValue)
                return result.Select(item => (KeyValuePair<string, long>)item).ToList();
            else
                return result.Select(item => item.ToString()).ToList();
        }
        return result;
    }

    private static void AppendAggtype(SimpleAggregateRequestDto requestDto, StringBuilder sql, StringBuilder append, string name, out bool isScalar)
    {
        isScalar = false;
        switch (requestDto.Type)
        {
            case AggregateTypes.Avg:
                sql.Append($"AVG({name}) as a");
                isScalar = true;
                break;
            case AggregateTypes.Count:
                sql.Append($"Count({name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.DistinctCount:
                sql.Append($"Count(DISTINCT {name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.Sum:
                sql.Append($"SUM({name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.GroupBy:
                sql.Append($"{name} as a,Count({name})  as b");
                append.Append($" and a<>'' Group By a order by b desc");
                break;
            case AggregateTypes.DateHistogram:
                sql.Append($"toStartOfInterval({name}, INTERVAL {ConvertInterval(requestDto.Interval)} minute ) as `time`,count() as `count`");
                append.Append($" Group by `time` order by `time`");
                break;
        }
    }

    public static int ConvertInterval(string s)
    {
        var unit = Regex.Replace(s, @"\d+", "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
        int t = 1;
        switch (unit)
        {
            case "s":
                t = 1;
                break;
            case "m":
                t = 60;
                break;
            case "h":
                t = 3600;
                break;
            case "d":
                t = 3600 * 24;
                break;
            case "w":
                t = 3600 * 24 * 7;
                break;
            case "month":
                t = 3600 * 24 * 30;
                break;
        }
        var num = Convert.ToInt64(s.Replace(unit, ""));
        num *= t;
        if (num - 60 < 0)
            return 1;
        return (int)(num / 60);
    }

    public static string GetMaxDelayTraceId(this IDbConnection dbConnection, BaseRequestDto requestDto)
    {
        var (where, parameters, _) = AppendWhere(requestDto);
        var text = $"select * from( {StorageConst.Current.TraceId} from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where} order by {StorageConst.Current.Trace.Duration} desc) as t limit 1";
        return dbConnection.ExecuteScalar(text, parameters?.ToArray())?.ToString()!;
    }
}
