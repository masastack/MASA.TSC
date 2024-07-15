// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal partial class ClickhouseApmService : IApmService
{
    private readonly MasaStackClickhouseConnection _dbConnection;
    private readonly ClickHouseCommand command;
    private readonly ITraceService _traceService;
    private readonly static object lockObj = new();
    private static readonly Dictionary<string, string> serviceOrders = new() {
        {nameof(ServiceListDto.Name),"ServiceName"},
        {nameof(ServiceListDto.Envs),"Resource.service.namespace"},
        {nameof(ServiceListDto.Latency),"Latency"},
        {nameof(ServiceListDto.Throughput),"Throughput"},
        {nameof(ServiceListDto.Failed),"Failed"},
    };

    private static readonly Dictionary<string, string> endpointOrders = new() {
        {nameof(EndpointListDto.Name),"Attributes.http.target"},
        {nameof(EndpointListDto.Service),"ServiceName"},
        {nameof(EndpointListDto.Method),"Attributes.http.method"},
        {nameof(EndpointListDto.Latency),"Latency"},
        {nameof(EndpointListDto.Throughput),"Throughput"},
        {nameof(EndpointListDto.Failed),"Failed"},
    };

    private static readonly Dictionary<string, string> errorOrders = new() {
        {nameof(ErrorMessageDto.Type),"Type"},
        {nameof(ErrorMessageDto.Message),"Message"},
        {nameof(ErrorMessageDto.LastTime),"time"},
        {nameof(ErrorMessageDto.Total),"total"}
    };
    const double MILLSECOND = 1e6;

    private readonly ILogger _logger;

    public ClickhouseApmService(MasaStackClickhouseConnection dbConnection, ITraceService traceService, ILogger<ClickhouseApmService> logger)
    {
        _traceService = traceService;
        _dbConnection = dbConnection;
        command = dbConnection.CreateCommand();
        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
        _logger = logger;
    }

    public Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query)
    {
        return MetricListAsync<ServiceListDto>(query, false);
    }

    public Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query)
    {
        return MetricListAsync<EndpointListDto>(query, true);
    }

    public Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query)
    {
        query.IsTrace = true;
        var (where, ors, parameters) = AppendWhere(query);
        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        var result = new EndpointLatencyDistributionDto();

        var p95 = Convert.ToDouble(Scalar($"select floor(quantileMerge(P95)/{MILLSECOND}) p95 from {tableName} where {where}", parameters));
        if (p95 is not double.NaN)
            result.P95 = (long)Math.Floor(p95);

        var sql = $@"select Duration/{MILLSECOND},count(1) total from {Constants.DurationCountTable1} where {where} group by Duration order by Duration";
        var list = new List<ChartPointDto>();
        //lock (lockObj)
        {
            using var reader = Query(sql, parameters);
            while (reader.NextResult())
                while (reader.Read())
                {
                    var item = new ChartPointDto()
                    {
                        X = reader[0].ToString()!,
                        Y = reader[1]?.ToString()!
                    };
                    list.Add(item);
                }
        }
        result.Latencies = list;
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsError = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = $"group by Type,Message{(string.IsNullOrEmpty(query.Endpoint) ? "" : ",Attributes.http.target")}";
        var append = string.IsNullOrEmpty(query.Endpoint) ? "" : ",Attributes.http.target";
        var combineSql = CombineOrs($"select Attributes.exception.type,MsgGroupKey,Timestamp{append} from {Constants.ErrorTable} where {where} ", ors);
        var countSql = $"select count(1) from (select `Attributes.exception.type` as Type,MsgGroupKey as Message,max(Timestamp) time,count(1) from {combineSql} {groupby})";
        PaginatedListBase<ErrorMessageDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, errorOrders);
        var sql = $@"select * from( select `Attributes.exception.type` as Type,MsgGroupKey as Message,max(Timestamp) time,count(1) total from {combineSql} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new ErrorMessageDto()
        {
            Type = reader[0]?.ToString()!,
            Message = reader[1]?.ToString()!,
            LastTime = Convert.ToDateTime(reader[2])!,
            Total = Convert.ToInt32(reader[3]),
        });
        return Task.FromResult(result);
    }

    public Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.LogTable} where {where} and SeverityText='Error' and `Attributes.exception.message`!='' {groupby}";

        return Task.FromResult(GetChartCountData(sql, parameters, query.ComparisonType).AsEnumerable());
    }

    public Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = false;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where} {groupby}";

        return Task.FromResult(GetChartCountData(sql, parameters, query.ComparisonType).AsEnumerable());
    }

    public Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.LogTable} where {where} {groupby}";
        return Task.FromResult(GetChartCountData(sql, parameters, query.ComparisonType).AsEnumerable());
    }

    public Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsError = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `SpanId` order by `SpanId`";
        var sql = CombineOrs($@"select SpanId from {Constants.ErrorTable} where {where}", ors);
        sql = $"select SpanId,count(1) `total` from ({sql}) {groupby}";
        var list = new List<ChartPointDto>();
        //lock (lockObj)
        {
            using var reader = Query(sql, parameters);
            while (reader.NextResult())
                while (reader.Read())
                {
                    var item = new ChartPointDto()
                    {
                        X = reader[0].ToString()!,
                        Y = reader[1]?.ToString()!
                    };
                    list.Add(item);
                }
        }
        return Task.FromResult(list.AsEnumerable());
    }

    public Dictionary<string, List<string>> GetEnviromentServices(BaseApmRequestDto query)
    {
        query.IsServer = true;
        query.IsMetric = true;
        var period = GetPeriod(query);
        var result = new Dictionary<string, List<string>>();
        var tableName = Constants.GetAggregateTable(period);
        var sql = $@"select        
        ServiceName,`Resource.service.namespace`
            from {tableName}
            where Timestamp between @start and @end
            group by `Resource.service.namespace`,ServiceName
            order by `Resource.service.namespace`,ServiceName";
        //lock (lockObj)
        {
            using var reader = Query(sql, new ClickHouseParameter[] {
                new (){ ParameterName="start",DbType= DbType.DateTime,Value=MasaStackClickhouseConnection.ToTimeZone(query.Start)},
                new (){ ParameterName="end",DbType= DbType.DateTime,Value=MasaStackClickhouseConnection.ToTimeZone(query.End)},
            });
            if (reader != null && !reader.NextResult())
                return result;
            string env = default!, currentEnv = default!;
            while (reader!.Read())
            {
                env = reader[1].ToString()!;
                var service = reader[0].ToString()!;
                if (currentEnv == null || currentEnv != env)
                {
                    result.Add(env, new List<string> { service });
                    currentEnv = env;
                }
                else
                {
                    result[env].Add(service);
                }
            }

            return result;
        }
    }

    public Task<PhoneModelDto> GetDeviceModelAsync(string brand, string model)
    {
        var isCodeAlis = brand.ToLower().Equals("apple") && model.Contains(',');
        var sql = $"select * from {Constants.ModelsTable} where lower(Brand)=@brand and  {(isCodeAlis ? "lower(CodeAlis)=@model" : "lower(Model)=@model")} limit 1";
        PhoneModelDto result = default!;
        //lock (lockObj)
        {
            using var reader = Query(sql, new ClickHouseParameter[] {
                new (){ ParameterName="brand",Value=brand.ToLower() },
                new (){ ParameterName="model",Value=model.ToLower() }
            });
            while (reader.NextResult())
                while (reader.Read())
                {
                    result = new PhoneModelDto()
                    {
                        Model = reader[0].ToString()!,
                        Type = reader[1].ToString()!,
                        Brand = reader[2]?.ToString()!,
                        BrandName = reader[3]?.ToString()!,
                        Code = reader[4]?.ToString()!,
                        CodeAlis = reader[5]?.ToString()!,
                        ModeName = reader[6]?.ToString()!,
                        VerName = reader[7]?.ToString()!,
                    };
                }
        }

        return Task.FromResult(result)!;
    }

    public async Task<PaginatedListBase<TraceResponseDto>> TraceLatencyDetailAsync(ApmTraceLatencyRequestDto query)
    {
        var queryDto = new BaseRequestDto
        {
            Start = query.Start,
            End = query.End,
            Endpoint = query.Endpoint,
            Service = query.Service!
        };
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(query.Env))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = StorageConstaaa.Current.Environment,
                Type = ConditionTypes.Equal,
                Value = query.Env
            });
        }

        if (query.LatMin.HasValue && query.LatMin.Value >= 0)
        {
            conditions.Add(new FieldConditionDto
            {
                Name = StorageConstaaa.Current.Trace.Duration,
                Type = ConditionTypes.GreatEqual,
                Value = (long)(query.LatMin.Value * MILLSECOND),
            });
        }

        if (query.LatMax.HasValue && query.LatMax.Value >= 0 && (
            !query.LatMin.HasValue
            || query.LatMin.HasValue && query.LatMax - query.LatMin.Value > 0))
            conditions.Add(new FieldConditionDto
            {
                Name = StorageConstaaa.Current.Trace.Duration,
                Type = ConditionTypes.LessEqual,
                Value = (long)(query.LatMax.Value * MILLSECOND),
            });
        if (conditions.Count > 0)
            queryDto.Conditions = conditions;

        return await _traceService.ListAsync(queryDto);
    }

    public Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceListAsync(ApmEndpointRequestDto query)
    {
        //query.IsServer = default;
        //query.IsTrace = true;
        var (where, ors, parameters) = AppendWhere(query);
        var orderBy = GetOrderBy(query, new());

        PaginatedListBase<SimpleTraceListDto> result = new() { };
        if (query.HasPage)
        {
            //var sql1 = CombineOrs($@"select TraceId from {Constants.DurationTable} where {where}", ors);
            //var countSql = $"select count(1) from {sql1}";
            var sql1 = CombineOrs($@"select countMerge(Total) as Total from {Constants.DurationCountTable1} where {where}", ors);
            var countSql = $"select sum(Total) from({sql1})";
            result.Total = Convert.ToInt64(Scalar(countSql, parameters));
        }
        //Constants.DurationTable

        var sql = CombineOrs($@"select TraceId,Duration,Timestamp from {Constants.DurationTable} where {where}", ors);
        sql = $"select TraceId,Duration,Timestamp from {sql} {orderBy} @limit";

        SetData(sql, parameters, result, query, ToSampleTraceListDto);
        return Task.FromResult(result);
    }

    private static SimpleTraceListDto ToSampleTraceListDto(IDataReader reader)
    {
        var startTime = Convert.ToDateTime(reader[StorageConstaaa.Current.Timestimap]);
        long ns = Convert.ToInt64(reader["Duration"]);
        var result = new SimpleTraceListDto
        {
            TraceId = reader[StorageConstaaa.Current.TraceId].ToString()!,
            Timestamp = startTime,
            EndTimestamp = startTime.AddMilliseconds(ns / 1e6),
        };
        return result;
    }

    private Task<PaginatedListBase<T>> MetricListAsync<T>(BaseApmRequestDto query, bool isEndpoint) where T : ServiceListDto, new()
    {
        query.IsServer = true;
        query.IsTrace = true;
        bool isInstrument = !string.IsNullOrEmpty(query.Queries);
        query.IsMetric = !isInstrument;
        var (where, ors, parameters) = AppendWhere(query);
        string countSql, sql;

        string groupAppend = isEndpoint ? ",`Attributes.http.target`,`Attributes.http.method`" : string.Empty;
        var groupby = $"group by ServiceName{groupAppend}";
        var orderBy = GetOrderBy(query, isEndpoint ? endpointOrders : serviceOrders, defaultSort: isEndpoint ? endpointOrders["Name"] : serviceOrders["Name"]);

        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        if (isInstrument)
        {
            var sql2 = CombineOrs($"select DISTINCT  ServiceName{groupAppend} from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors);
            sql = @$"select a.* from(select
                    ServiceName,Resource.service.namespace,
                    avgMerge(Latency) as Latency,
                    countMerge(Throughput) as Throughput,
                    SumMerge(Failed) as Failed {groupAppend}
            from {tableName}
            where {where}
            group by ServiceName,`Resource.service.namespace`{groupAppend},Timestamp) a,
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.Attributes.http.target=b.Attributes.http.target and a.Attributes.http.method=b.Attributes.http.method" : "")}";
            countSql = @$"select a.* from(select
                    ServiceName{groupAppend}
            from {tableName}
            where {where}
            group by ServiceName{groupAppend}) a,
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.Attributes.http.target=b.Attributes.http.target and a.Attributes.http.method=b.Attributes.http.method" : "")}";
        }
        else
        {
            sql = @$"select
                    ServiceName,Resource.service.namespace,
                    avgMerge(Latency) as Latency,
                    countMerge(Throughput) as Throughput,
                    SumMerge(Failed) as Failed {groupAppend}
            from {tableName}
            where {where}
            group by ServiceName,`Resource.service.namespace`{groupAppend},Timestamp";
            countSql = $"select count(1) from {tableName} where {where} {groupby}";
        }

        countSql = $"select count(1) from ({countSql})";
        sql = $@"select 
    ServiceName,
`Resource.service.namespace1` as `Resource.service.namespace`,Latency1 as Latency,
Throughput1 as Throughput,Failed1 as Failed {groupAppend}
from(
        select
        ServiceName,
        arrayStringConcat(groupUniqArray(`Resource.service.namespace`)) AS `Resource.service.namespace1`,
        floor(sum(Latency*Throughput)/sum(Throughput)/{MILLSECOND}) as Latency1,
        sum(Throughput) as Throughput1,
        round(sum(Failed)*100/if(sum(Throughput)=0,1,sum(Throughput)),2) as Failed1 {groupAppend}
        from({sql}) t
        {groupby}
        ) {orderBy} @limit ";
        PaginatedListBase<T> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        SetData(sql, parameters, result, query, reader => ToServiceList<T>(reader));
        return Task.FromResult(result);
    }

    private static T ToServiceList<T>(IDataReader reader) where T : ServiceListDto, new()
    {
        var result = new T()
        {
            Service = reader[0].ToString()!,
            Envs = reader[1]?.ToString()?.Split(',') ?? Array.Empty<string>(),
            Latency = (long)Math.Floor(Convert.ToDouble(reader[2])),
            Throughput = Math.Round(Convert.ToDouble(reader[3]), 2),
            Failed = Math.Round(Convert.ToDouble(reader[4]), 2),
        };
        if (result is EndpointListDto endpointListDto)
        {
            endpointListDto.Endpoint = reader[5].ToString()!;
            endpointListDto.Method = reader[6].ToString()!;
        }
        return result;
    }

    private void GetPreviousChartData(BaseApmRequestDto query, string sql, List<ClickHouseParameter> parameters, List<ChartLineDto> result)
    {
        if (!query.ComparisonType.HasValue)
            return;

        int day = 0;
        switch (query.ComparisonType.Value)
        {
            case ComparisonTypes.DayBefore:
                day = -1;
                break;
            case ComparisonTypes.WeekBefore:
                day = -7;
                break;
        }
        if (day == 0)
            return;

        var paramStartTime = parameters.First(p => p.ParameterName == "startTime");
        paramStartTime.Value = ((DateTime)paramStartTime.Value!).AddDays(day);

        var paramEndTime = parameters.First(p => p.ParameterName == "endTime");
        paramEndTime.Value = ((DateTime)paramEndTime.Value!).AddDays(day);

        //lock (lockObj)
        {
            using var readerPrevious = Query(sql, parameters);
            SetChartData(result, readerPrevious, isPrevious: true);
        }
    }

    private static void SetChartData(List<ChartLineDto> result, IDataReader reader, bool isPrevious = false)
    {
        if (!reader.NextResult())
            return;
        ChartLineDto? current = null;
        while (reader.Read())
        {
            string name;
            if (reader.FieldCount - 7 == 0)//service
            {
                name = reader[6].ToString()!;
            }
            else
            {
                name = $"{reader[8]} {reader[7]}";
            }
            var time = new DateTimeOffset(Convert.ToDateTime(reader[0])).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineDto
                    {
                        Name = name,
                        Previous = new List<ChartLineItemDto>(),
                        Currents = new List<ChartLineItemDto>()
                    };
                    result.Add(current);
                }
            }

            ((List<ChartLineItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Latency = (long)Math.Floor(Convert.ToDouble(reader[1])),
                    Throughput = Math.Round(Convert.ToDouble(reader[2]), 2, MidpointRounding.ToZero),
                    Failed = Math.Round(Convert.ToDouble(reader[3]), 2, MidpointRounding.ToZero),
                    P99 = Math.Round(Convert.ToDouble(reader[4]), 2, MidpointRounding.ToZero),
                    P95 = Math.Round(Convert.ToDouble(reader[5]), 2, MidpointRounding.ToZero),
                    Time = time
                });
        }
    }

    public Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query)
    {
        query.IsServer = true;
        query.IsTrace = true;
        bool isInstrument = !string.IsNullOrEmpty(query.Queries);
        query.IsMetric = !isInstrument;
        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        var (where, ors, parameters) = AppendWhere(query);
        bool isEndpoint = query is ApmEndpointRequestDto;
        string groupAppend = isEndpoint ? ",`Attributes.http.target`,`Attributes.http.method`" : string.Empty;
        string sql;

        var result = new List<ChartLineDto>();
        if (isInstrument)
        {
            var sql2 = CombineOrs($"select DISTINCT  ServiceName{groupAppend} from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors);
            sql = @$"select a.* from(select  
                    Timestamp,
                    avgMerge(Latency) as Latency,
                    countMerge(Throughput) as Throughput,
                    SumMerge(Failed) as Failed,
                     floor(quantileMerge(P99)/{MILLSECOND}) as P99,
                    floor(quantileMerge(P95)/{MILLSECOND}) as P95,
                    ServiceName
                    {groupAppend}
            from {tableName}
            where {where}
            group by ServiceName{groupAppend},Timestamp) a,
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.Attributes.http.target=b.Attributes.http.target and a.Attributes.http.method=b.Attributes.http.method" : "")}
            order by ServiceName{groupAppend},Timestamp";
        }
        else
        {
            sql = $@"select        
        Timestamp,
        floor(Latency1/{MILLSECOND}) as Latency,
        Throughput1 as Throughput,
        round(Failed1*100/if(Throughput1=0,1,Throughput1),2) as Failed,
        floor(P991/{MILLSECOND}) as P99,
        floor(P951/{MILLSECOND}) as P95,
        ServiceName{groupAppend}
        from(
            select                    
                    Timestamp,
                    avgMerge(Latency) as Latency1,
                    countMerge(Throughput) as Throughput1,
                    SumMerge(Failed) as Failed1,
                    quantileMerge(P99) P991,
                    quantileMerge(P95) P951,
                    ServiceName{groupAppend}
            from {tableName}
            where {where}
            group by ServiceName{groupAppend},Timestamp
            order by ServiceName{groupAppend},Timestamp
            ) t";
        }
        //lock (lockObj)
        {
            using var reader = Query(sql, parameters);
            SetChartData(result, reader);
        }
        GetPreviousChartData(query, sql, parameters, result);
        return Task.FromResult(result.AsEnumerable());
    }

    private void SetData<TResult>(string sql, List<ClickHouseParameter> parameters, PaginatedListBase<TResult> result, BaseApmRequestDto query, Func<IDataReader, TResult> parseFn) where TResult : class
    {
        var start = (query.Page - 1) * query.PageSize;
        if (result.Total - start > 0)
        {
            //lock (lockObj)
            {
                using var reader = Query(sql.Replace("@limit", $"limit {start},{query.PageSize}"), parameters);
                result.Result = new();
                while (reader.NextResult())
                    while (reader.Read())
                        result.Result.Add(parseFn(reader));
            }
        }
    }

    private static (string where, List<string> ors, List<ClickHouseParameter> parameters) AppendWhere<TQuery>(TQuery query) where TQuery : BaseApmRequestDto
    {
        List<ClickHouseParameter> parameters = new();
        var sql = new StringBuilder();
        sql.AppendLine($" {StorageConstaaa.Current.Timestimap} between @startTime and @endTime");
        parameters.Add(new ClickHouseParameter { ParameterName = "startTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime });
        parameters.Add(new ClickHouseParameter { ParameterName = "endTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime });
        if (!string.IsNullOrEmpty(query.Env))
        {
            sql.AppendLine($" and {StorageConstaaa.Current.Environment}=@environment");
            parameters.Add(new ClickHouseParameter { ParameterName = "environment", Value = query.Env });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.AppendLine($" and {StorageConstaaa.Current.ServiceName}=@serviceName");
            parameters.Add(new ClickHouseParameter { ParameterName = "serviceName", Value = query.Service });
        }
        //if (query.IsServer.HasValue && (query.IsMetric == null || !query.IsMetric.Value))
        //{
        //    sql.AppendLine(" and SpanKind=@spanKind");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "spanKind", Value = query.IsServer.Value ? "SPAN_KIND_SERVER" : "SPAN_KIND_CLIENT" });
        //}
        AppendEndpoint(query as ApmEndpointRequestDto, sql, parameters);
        AppendDuration(query as ApmTraceLatencyRequestDto, sql, parameters);
        var ors = AppendRawQuery(query, parameters);
        return (sql.ToString(), ors, parameters);
    }

    private static List<string> AppendRawQuery<TQuery>(TQuery query, List<ClickHouseParameter> parameters) where TQuery : BaseApmRequestDto
    {
        var list = new List<string>();
        if (string.IsNullOrEmpty(query.Queries) || query.Queries.Trim().Length == 0)
            return list;
        //sql
        if (query.Queries.Contains(" and ") || query.Queries.Contains(" or ") || query.Queries.Contains("='") || query.Queries.Contains(" in ") || query.Queries.Contains(" not in ") || query.Queries.Contains(" like ") || query.Queries.Contains(" not like  "))
        {
            list.Add(!query.Queries.Trim().StartsWith("and ", StringComparison.CurrentCultureIgnoreCase) ? $" and {query.Queries}" : query.Queries);
            return list;
        }

        bool isTrace = query.IsTrace ?? default, isLog = query.IsLog ?? default, isError = query.IsError ?? default;
        if (!(isTrace || isLog || isError))
            return list;
        var str = query.Queries.Trim();

        if (Guid.TryParse(str, out var _))
        {
            if (str.Length - 32 == 0)
            {
                list.Add($" and {StorageConstaaa.Current.TraceId}='{str}'");
            }
            else
            {
                list.Add($" and {StorageConstaaa.Current.Trace.UserId}='{str}'");
            }
            return list;
        }

        //status_code
        if (int.TryParse(str, out int num) && num - -1 >= 0 && num - 600 < 0 && isTrace)
        {
            list.Add($" and {StorageConstaaa.Current.Trace.HttpStatusCode}='{query.Queries}'");
            return list;
        }

        //exception
        if (str.EndsWith("exception", StringComparison.CurrentCultureIgnoreCase) && Regex.IsMatch(str, @"[\da-zA-Z]+"))
        {
            list.Add($" and {StorageConstaaa.Current.ExceptionType} = '{str}'");
            return list;
        }

        if (isLog)
        {
            list.Add(ClickhouseHelper.AppendLike(StorageConstaaa.Current.Log.Body, "p1", str));

        }
        else if (isTrace)
        {
            list.Add(ClickhouseHelper.AppendLike(StorageConstaaa.Current.Trace.URL, "p1", str));
            list.Add(ClickhouseHelper.AppendLike(StorageConstaaa.Current.Trace.HttpRequestBody, "p1", str));
        }

        if (!isTrace)
            list.Add(ClickhouseHelper.AppendLike(StorageConstaaa.Current.ExceptionMessage, "p1", str));

        parameters.Add(ClickhouseHelper.GetLikeParameter("p1", str));
        return list;
    }

    private static string CombineOrs(string sql, IEnumerable<string> ors, string? orderBy = null, string? groupBy = null)
    {
        if (ors == null || !ors.Any())
            return $"({sql} {groupBy} {orderBy})";

        var text = new StringBuilder();
        foreach (var or in ors)
        {
            text.AppendLine($" union all {sql}{or} {groupBy} {orderBy}");
        }
        text.Remove(0, 11).Insert(0, '(').Append(')');
        return text.ToString();
    }

    private static void AppendEndpoint(ApmEndpointRequestDto? traceQuery, StringBuilder sql, List<ClickHouseParameter> parameters)
    {
        if (traceQuery == null || string.IsNullOrEmpty(traceQuery.Endpoint))
            return;
        var name = "endpoint";
        if (traceQuery.IsLog.HasValue && traceQuery.IsLog.Value)
        {
            sql.AppendLine($" and {ClickhouseHelper.GetName(StorageConstaaa.Current.Log.Url, true)} like @{name}");
            parameters.Add(new ClickHouseParameter { ParameterName = name, Value = $"{traceQuery.Endpoint}%" });
        }
        else
        {
            sql.AppendLine($" and {StorageConstaaa.Current.Trace.URL}=@{name}");
            parameters.Add(new ClickHouseParameter { ParameterName = name, Value = traceQuery.Endpoint });
        }
    }

    private static void AppendDuration(ApmTraceLatencyRequestDto? query, StringBuilder sql, List<ClickHouseParameter> parameters)
    {
        if (query == null || !query.LatMin.HasValue && !query.LatMax.HasValue || query.IsMetric != null && query.IsMetric.Value) return;
        if (query.LatMin.HasValue && query.LatMin > 0)
        {
            sql.AppendLine($" and {StorageConstaaa.Current.Trace.Duration} >=@minDuration");
            parameters.Add(new ClickHouseParameter { ParameterName = "minDuration", Value = (long)(query.LatMin * MILLSECOND) });
        }
        if (query.LatMax.HasValue && query.LatMax > 0)
        {
            sql.AppendLine($" and {StorageConstaaa.Current.Trace.Duration} <=@maxDuration");
            parameters.Add(new ClickHouseParameter { ParameterName = "maxDuration", Value = (long)(query.LatMax * MILLSECOND) });
        }
    }

    private IDataReader Query(string sql, IEnumerable<ClickHouseParameter> parameters)
    {
        var start = DateTime.Now;
        try
        {
            command.CommandText = sql;
            SetParameters(parameters);
            return command.ExecuteReader();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "execute sql error:{Sqlraw}", sql);
            throw;
        }
        finally
        {
            var end = DateTime.Now;
            var duration = (end - start).TotalSeconds;
            if (duration - 1 > 0)
                _logger.LogWarning("Clickhouse query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}", duration, sql, parameters);
        }
    }

    private object Scalar(string sql, IEnumerable<ClickHouseParameter> parameters)
    {
        //lock (lockObj)
        {
            command.CommandText = sql;
            SetParameters(parameters);
            var start = DateTime.Now;
            try
            {
                return command.ExecuteScalar()!;
            }
            finally
            {
                var end = DateTime.Now;
                var duration = (end - start).TotalSeconds;
                if (duration - 1 > 0)
                    _logger.LogWarning("Clickhouse query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}", duration, sql, parameters);
            }
        }
    }

    private void SetParameters(IEnumerable<ClickHouseParameter> parameters)
    {
        if (command.Parameters.Count > 0)
            command.Parameters.Clear();
        if (parameters != null && parameters.Any())
            foreach (var param in parameters)
                command.Parameters.Add(param);
    }

    private static string? GetOrderBy(BaseApmRequestDto query, Dictionary<string, string> sortFields, string? defaultSort = null, bool isDesc = false)
    {
        if (!string.IsNullOrEmpty(query.OrderField) && sortFields.TryGetValue(query.OrderField, out var field))
        {
            if (!query.IsDesc.HasValue)
                return $" order by `{field}`";
            return $" order by `{field}`{(query.IsDesc.Value ? " desc" : "")}";
        }

        if (string.IsNullOrEmpty(defaultSort))
            return null;
        return $" order by `{defaultSort}`{(isDesc ? " desc" : "")}";
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
    }

    private List<ChartLineCountDto> GetChartCountData(string sql, IEnumerable<ClickHouseParameter> parameters, ComparisonTypes? comparisonTypes = null)
    {
        var result = new List<ChartLineCountDto>();
        //lock (lockObj)
        {
            var t1 = DateTime.Now;
            using var currentReader = Query(sql, parameters);
            var t2 = DateTime.Now;
            var ta = (t2 - t1).TotalSeconds;
            SetChartCountData(result, currentReader);
            var t3 = DateTime.Now;
            var tb = (t3 - t2).TotalSeconds;
        }

        if (comparisonTypes.HasValue && (comparisonTypes.Value == ComparisonTypes.DayBefore || comparisonTypes.Value == ComparisonTypes.WeekBefore))
        {
            var day = comparisonTypes.Value == ComparisonTypes.DayBefore ? -1 : -7;
            var paramStartTime = parameters.First(p => p.ParameterName == "startTime");
            paramStartTime.Value = ((DateTime)paramStartTime.Value!).AddDays(day);

            var paramEndTime = parameters.First(p => p.ParameterName == "endTime");
            paramEndTime.Value = ((DateTime)paramEndTime.Value!).AddDays(day);

            //lock (lockObj)
            {
                var t1 = DateTime.Now;
                using var previousReader = Query(sql, parameters);
                var t2 = DateTime.Now;
                var ta = (t2 - t1).TotalSeconds;
                SetChartCountData(result, previousReader, true);
                var t3 = DateTime.Now;
                var tb = (t3 - t2).TotalSeconds;
            }
        }

        return result;
    }

    private void SetChartCountData(List<ChartLineCountDto> result, IDataReader reader, bool isPrevious = false)
    {
        var t1 = DateTime.Now;
        if (!reader.NextResult())
            return;
        ChartLineCountDto? current = null;
        while (reader.Read())
        {
            var name = reader.GetValue(0).ToString()!;
            var time = new DateTimeOffset(Convert.ToDateTime(reader.GetValue(0))).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineCountDto
                    {
                        Name = name,
                        Previous = new List<ChartLineCountItemDto>(),
                        Currents = new List<ChartLineCountItemDto>()
                    };
                    result.Add(current);
                }
            }

            ((List<ChartLineCountItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Value = reader.GetValue(1),
                    Time = time
                });
        }
        var t2 = DateTime.Now;
        var tb = (t2 - t1).TotalSeconds;
    }

    private static string GetPeriod(BaseApmRequestDto query)
    {
        var reg = new Regex(@"/d+", default, TimeSpan.FromSeconds(5));
        if (string.IsNullOrEmpty(query.Period) || !reg.IsMatch(query.Period))
        {
            return GetDefaultPeriod(query.End - query.Start);
        }
        var unit = reg.Replace(query.Period, "").Trim().ToLower();
        var units = new List<string> { "year", "month", "week", "day", "hour", "minute", "second" };
        var find = units.Find(s => s.StartsWith(unit));
        if (string.IsNullOrEmpty(find))
            find = "minute";
        return $"{reg.Match(query.Period).Result} {find}";
    }

    private static string GetDefaultPeriod(TimeSpan timeSpan)
    {
        if ((int)timeSpan.TotalHours < 1)
        {
            return "1 minute";
        }

        var days = (int)timeSpan.TotalDays;
        if (days <= 0)
        {
            if ((int)timeSpan.TotalHours - 12 <= 0)
            {
                return "1 minute";
            }
            return "30 minute";
        }

        if (days - 7 <= 0)
        {
            return "1 hour";
        }

        if (days - 30 <= 0)
        {
            return "1 day";
        }

        if (days - 365 <= 0)
        {
            return "1 week";
        }

        return "1 month";
    }
}
