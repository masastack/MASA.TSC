// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal partial class ClickhouseApmServiceNew : IApmService
{
    private readonly MasaStackClickhouseConnection _dbConnection;
    private readonly ClickHouseCommand command;
    private readonly ITraceService _traceService;
    private static Dictionary<string, string> serviceOrders = default!;
    private static Dictionary<string, string> endpointOrders = default!;
    private static Dictionary<string, string> errorOrders = default!;
    const double MILLSECOND = 1e6;
    private readonly ILogger _logger;

    public ClickhouseApmServiceNew(MasaStackClickhouseConnection dbConnection, ITraceService traceService, ILogger<ClickhouseApmServiceNew> logger)
    {
        _traceService = traceService;
        _dbConnection = dbConnection;
        command = dbConnection.CreateCommand();
        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
        _logger = logger;

        serviceOrders ??= new() {
            { nameof(ServiceListDto.Name), StorageConst.Current.ServiceName },
            { nameof(ServiceListDto.Envs), StorageConst.Current.Environment },
            { nameof(ServiceListDto.Latency), "Latency" },
            { nameof(ServiceListDto.Throughput), "Throughput" },
            { nameof(ServiceListDto.Failed), "Failed" },
        };
        endpointOrders ??= new() {
            { nameof(EndpointListDto.Name), StorageConst.Current.Trace.URL },
            { nameof(EndpointListDto.Service), StorageConst.Current.ServiceName },
            { nameof(EndpointListDto.Method), StorageConst.Current.Trace.HttpMethod },
            { nameof(EndpointListDto.Latency), "Latency" },
            { nameof(EndpointListDto.Throughput), "Throughput" },
            { nameof(EndpointListDto.Failed), "Failed" },
        };
        errorOrders ??= new() {
            {nameof(ErrorMessageDto.Type),"Type"},
            {nameof(ErrorMessageDto.Message),"Message"},
            {nameof(ErrorMessageDto.LastTime),"time"},
            {nameof(ErrorMessageDto.Total),"total"}
        };
    }

    private void Log(DateTime start, DateTime end, string sql, IEnumerable<ClickHouseParameter> parameters, bool isReader = false)
    {
        var duration = (end - start).TotalSeconds;
        if (duration - 1 > 0)
            _logger.LogWarning("Clickhouse query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}, isReader:{IsReader}", duration, sql, parameters, isReader);
    }

    public Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query)
    {
        return MetricListAsync<ServiceListDto>(query, false);
    }

    public Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query)
    {
        return MetricListAsync<EndpointListDto>(query, true);
    }

    public async Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query)
    {
        query.IsTrace = true;
        var (where, parameters) = GetMetricWhere(query, true);
        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        var result = new EndpointLatencyDistributionDto();

        var p95 = Convert.ToDouble(await Scalar($"select floor(quantileMerge(P95)/{MILLSECOND}) p95 from {tableName} where {where}", parameters));
        if (p95 is not double.NaN)
            result.P95 = (long)Math.Floor(p95);

        var sql = $@"select Duration,count(1) total from {Constants.DurationCountTable} where {where} group by Duration order by Duration";
        var list = new List<ChartPointDto>();

        using var reader = await Query(sql, parameters);
        var start = DateTime.Now;
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
            {
                var item = new ChartPointDto()
                {
                    X = reader[0].ToString()!,
                    Y = reader[1]?.ToString()!
                };
                list.Add(item);
            }
        var end = DateTime.Now;
        Log(start, end, sql, parameters, true);

        result.Latencies = list;
        return result;
    }

    public async Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsError = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = $"group by Type,Message{(string.IsNullOrEmpty(query.Endpoint) ? "" : ",Attributes.http.target")}";
        var append = string.IsNullOrEmpty(query.Endpoint) ? "" : ",Attributes.http.target";
        var combineSql = CombineOrs($"select Attributes.exception.type,MsgGroupKey,Timestamp{append} from {Constants.ErrorTable} where {where} ", ors);
        var countSql = $"select count(1) from (select `Attributes.exception.type` as Type,MsgGroupKey as Message,max(Timestamp) time,count(1) from {combineSql} {groupby})";
        PaginatedListBase<ErrorMessageDto> result = new() { Total = Convert.ToInt64(await Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, errorOrders);
        var sql = $@"select * from( select `Attributes.exception.type` as Type,MsgGroupKey as Message,max(Timestamp) time,count(1) total from {combineSql} {groupby} {orderBy} @limit)";
        await SetData(sql, parameters, result, query, reader => new ErrorMessageDto()
        {
            Type = reader[0]?.ToString()!,
            Message = reader[1]?.ToString()!,
            LastTime = Convert.ToDateTime(reader[2])!,
            Total = Convert.ToInt32(reader[3]),
        });
        return result;
    }

    public async Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.LogTable} where {where} and SeverityText='Error' and `Attributes.exception.message`!='' {groupby}";

        return await GetChartCountData(sql, parameters, query.ComparisonType);
    }

    public async Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = false;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where} {groupby}";

        return await GetChartCountData(sql, parameters, query.ComparisonType);
    }

    public async Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {MasaStackClickhouseConnection.LogTable} where {where} {groupby}";
        return await GetChartCountData(sql, parameters, query.ComparisonType);
    }

    public async Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsError = true;
        var (where, ors, parameters) = AppendWhere(query);
        var groupby = "group by `SpanId` order by `SpanId`";
        var sql = CombineOrs($@"select SpanId from {Constants.ErrorTable} where {where}", ors);
        sql = $"select SpanId,count(1) `total` from ({sql}) {groupby}";
        var list = new List<ChartPointDto>();

        using var reader = await Query(sql, parameters);
        var start = DateTime.Now;
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
            {
                var item = new ChartPointDto()
                {
                    X = reader[0].ToString()!,
                    Y = reader[1]?.ToString()!
                };
                list.Add(item);
            }
        var end = DateTime.Now;
        Log(start, end, sql, parameters, true);

        return list;
    }

    public async Task<Dictionary<string, List<string>>> GetEnviromentServices(BaseApmRequestDto query)
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
            var parameters = new ClickHouseParameter[] {
                new (){ ParameterName="start",DbType= DbType.DateTime,Value=MasaStackClickhouseConnection.ToTimeZone(query.Start)},
                new (){ ParameterName="end",DbType= DbType.DateTime,Value=MasaStackClickhouseConnection.ToTimeZone(query.End)},
            };
            using var reader = await Query(sql, parameters);
            var start = DateTime.Now;
            if (reader == null)
                return result;
            string env = default!, currentEnv = default!;
            while (await reader.NextResultAsync())
                while (await reader.ReadAsync())
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
            var end = DateTime.Now;
            Log(start, end, sql, parameters, true);

            return result;
        }
    }

    public async Task<PhoneModelDto> GetDeviceModelAsync(string brand, string model)
    {
        var isCodeAlis = brand.ToLower().Equals("apple") && model.Contains(',');
        var sql = $"select * from {Constants.ModelsTable} where lower(Brand)=@brand and  {(isCodeAlis ? "lower(CodeAlis)=@model" : "lower(Model)=@model")} limit 1";
        PhoneModelDto result = default!;
        //lock (lockObj)
        {
            var parameters = new ClickHouseParameter[] {
                new (){ ParameterName="brand",Value=brand.ToLower() },
                new (){ ParameterName="model",Value=model.ToLower() }
            };
            using var reader = await Query(sql, parameters);
            var start = DateTime.Now;
            while (await reader.NextResultAsync())
                while (await reader.ReadAsync())
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
            var end = DateTime.Now;
            Log(start, end, sql, parameters, true);
        }

        return result!;
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
                Name = StorageConst.Current.Environment,
                Type = ConditionTypes.Equal,
                Value = query.Env
            });
        }

        if (query.LatMin.HasValue && query.LatMin.Value >= 0)
        {
            conditions.Add(new FieldConditionDto
            {
                Name = StorageConst.Current.Trace.Duration,
                Type = ConditionTypes.GreatEqual,
                Value = (long)(query.LatMin.Value * MILLSECOND),
            });
        }

        if (query.LatMax.HasValue && query.LatMax.Value >= 0 && (
            !query.LatMin.HasValue
            || query.LatMin.HasValue && query.LatMax - query.LatMin.Value > 0))
            conditions.Add(new FieldConditionDto
            {
                Name = StorageConst.Current.Trace.Duration,
                Type = ConditionTypes.LessEqual,
                Value = (long)(query.LatMax.Value * MILLSECOND),
            });
        if (conditions.Count > 0)
            queryDto.Conditions = conditions;

        return await _traceService.ListAsync(queryDto);
    }

    public async Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceListAsync(ApmEndpointRequestDto query)
    {
        var orderBy = GetOrderBy(query, new() { { StorageConst.Current.Timestimap, StorageConst.Current.Timestimap } });
        var (where, ors, parameters) = AppendWhere(query);
        PaginatedListBase<SimpleTraceListDto> result = new() { };
        if (query.HasPage)
        {
            string sql1;
            if (query.IsInstrument)
            {
                sql1 = $"select count(1) as Total from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}";
            }
            else
            {
                sql1 = $@"select countMerge(Total) as Total from {Constants.DurationCountTable} where {where}";
            }

            sql1 = CombineOrs(sql1, ors);           
            var countSql = $"select sum(Total) from({sql1})";
            result.Total = Convert.ToInt64(await Scalar(countSql, parameters));
        }
      
        var sql = CombineOrs($@"select TraceId,Duration,Timestamp from {(query.IsInstrument ? MasaStackClickhouseConnection.TraceHttpServerTable : Constants.DurationTable)} where {where}", ors);
        sql = $"select TraceId,Duration,Timestamp from {sql} {orderBy} @limit";

        await SetData(sql, parameters, result, query, ToSampleTraceListDto);
        return result;
    }   

    private static SimpleTraceListDto ToSampleTraceListDto(IDataReader reader)
    {
        var startTime = Convert.ToDateTime(reader[StorageConst.Current.Timestimap]);
        long ns = Convert.ToInt64(reader["Duration"]);
        var result = new SimpleTraceListDto
        {
            TraceId = reader[StorageConst.Current.TraceId].ToString()!,
            Timestamp = startTime,
            EndTimestamp = startTime.AddMilliseconds(ns / 1e6),
        };
        return result;
    }

    private async Task<PaginatedListBase<T>> MetricListAsync<T>(BaseApmRequestDto query, bool isEndpoint) where T : ServiceListDto, new()
    {
        query.IsServer = true;
        query.IsTrace = true;
        var (where, ors, parameters) = AppendWhere(query);
        var (metricWhere, _) = GetMetricWhere(query, true);
        string countSql, sql;

        string groupAppend = isEndpoint ? ",`Attributes.http.target`,`Attributes.http.method`" : string.Empty;
        var groupby = $"group by ServiceName{groupAppend}";
        var orderBy = GetOrderBy(query, isEndpoint ? endpointOrders : serviceOrders, defaultSort: isEndpoint ? endpointOrders["Name"] : serviceOrders["Name"]);

        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        if (query.IsInstrument)
        {
            var sql2 = CombineOrs($"select DISTINCT  ServiceName{groupAppend} from {MasaStackClickhouseConnection.TraceHttpServerTable} where {where}", ors);
            sql = @$"select a.* from(select
                    ServiceName,Resource.service.namespace,
                    avgMerge(Latency) as Latency,
                    countMerge(Throughput) as Throughput,
                    SumMerge(Failed) as Failed {groupAppend}
            from {tableName}
            where {metricWhere}
            group by ServiceName,`Resource.service.namespace`{groupAppend},Timestamp) a,
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.`Attributes.http.target`=b.`Attributes.http.target` and a.`Attributes.http.method`=b.`Attributes.http.method`" : "")}";
            countSql = @$"select a.* from(select
                    ServiceName{groupAppend}
            from {tableName}
            where {metricWhere}
            group by ServiceName{groupAppend}) a,
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.`Attributes.http.target`=b.`Attributes.http.target` and a.`Attributes.http.method`=b.`Attributes.http.method`" : "")}";
        }
        else
        {
            sql = @$"select
                    ServiceName,Resource.service.namespace,
                    avgMerge(Latency) as Latency,
                    countMerge(Throughput) as Throughput,
                    SumMerge(Failed) as Failed {groupAppend}
            from {tableName}
            where {metricWhere}
            group by ServiceName,`Resource.service.namespace`{groupAppend},Timestamp";
            countSql = $"select count(1) from {tableName} where {metricWhere} {groupby}";
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
        PaginatedListBase<T> result = new() { Total = Convert.ToInt64(await Scalar(countSql, parameters)) };
        await SetData(sql, parameters, result, query, reader => ToServiceList<T>(reader));
        return result;
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

    private async Task GetPreviousChartData(BaseApmRequestDto query, string sql, List<ClickHouseParameter> parameters, List<ChartLineDto> result)
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
            var readerPrevious = await Query(sql, parameters);
            await SetChartData(result, readerPrevious, isPrevious: true);
            await readerPrevious.DisposeAsync();
        }
    }

    private async Task SetChartData(List<ChartLineDto> result, DbDataReader reader, bool isPrevious = false)
    {
        ChartLineDto? current = null;
        var start = DateTime.Now;
        while (reader != null && await reader.NextResultAsync())
            while (reader != null && await reader.ReadAsync())
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
        var end = DateTime.Now;
        Log(start, end, default, default, true);
    }

    public async Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query)
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
            (select DISTINCT ServiceName{groupAppend} from ({sql2})) b where a.ServiceName=b.ServiceName{(isEndpoint ? " and a.`Attributes.http.target`=b.`Attributes.http.target` and a.`Attributes.http.method`=b.`Attributes.http.method`" : "")}
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
            using (var reader = await Query(sql, parameters))
                await SetChartData(result, reader);
        }
        await GetPreviousChartData(query, sql, parameters, result);
        return result;
    }

    public async Task<List<string>> GetStatusCodesAsync()
    {
        var sql = $@"select  `Attributes.http.status_code` from
{MasaStackClickhouseConnection.TraceHttpServerTable}
group by  `Attributes.http.status_code`
order by `Attributes.http.status_code`";
        var result = new List<string>();
        using var reader = await Query(sql, default!);
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
                result.Add(reader[0].ToString()!);

        return result;
    }

    public async Task<List<string>> GetEndpointsAsync(BaseApmRequestDto query)
    {
        var period = GetPeriod(query);
        var tableName = Constants.GetAggregateTable(period);
        var (where, ors, parameters) = AppendWhere(query);
        var sql = @$"select {StorageConst.Current.Trace.URL}
            from {tableName}
            where {where}
            group by ServiceName,{StorageConst.Current.Trace.URL}
            order by {StorageConst.Current.Trace.URL}";
        var result = new List<string>();
        using var reader = await Query(sql, parameters);
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
                result.Add(reader[0].ToString()!);

        return result;
    }

    public async Task<List<string>> GetErrorTypesAsync(BaseApmRequestDto query)
    {
        var (where, ors, parameters) = AppendWhere(query);
        var sql = $@"select `Attributes.exception.type` from {Constants.ErrorTable} where {where} group by `Attributes.exception.type` order by `Attributes.exception.type`";
        var result = new List<string>();
        using var reader = await Query(sql, parameters);
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
                result.Add(reader[0].ToString()!);

        return result;
    }

    private async Task SetData<TResult>(string sql, List<ClickHouseParameter> parameters, PaginatedListBase<TResult> result, BaseApmRequestDto query, Func<IDataReader, TResult> parseFn) where TResult : class
    {
        var start = (query.Page - 1) * query.PageSize;
        if (result.Total - start > 0)
        {
            //lock (lockObj)

            using (var reader = await Query(sql.Replace("@limit", $"limit {start},{query.PageSize}"), parameters))
            {
                result.Result = new();
                var _start = DateTime.Now;
                while (await reader.NextResultAsync())
                    while (await reader.ReadAsync())
                        result.Result.Add(parseFn(reader));
                var end = DateTime.Now;
                Log(_start, end, sql, parameters, true);
            }
        }
    }

    private static (string where, List<string> ors, List<ClickHouseParameter> parameters) AppendWhere<TQuery>(TQuery query) where TQuery : BaseApmRequestDto
    {
        (string where, List<ClickHouseParameter> parameters) = GetMetricWhere(query);
        var sql = new StringBuilder(where);
        var ors = new List<string>();

        //sql.AppendLine($" {StorageConstaaa.Current.Timestimap} between @startTime and @endTime");
        //parameters.Add(new ClickHouseParameter { ParameterName = "startTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime });
        //parameters.Add(new ClickHouseParameter { ParameterName = "endTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime });
        //if (!string.IsNullOrEmpty(query.Env))
        //{
        //    sql.AppendLine($" and {StorageConstaaa.Current.Environment}=@environment");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "environment", Value = query.Env });
        //}
        //if (!string.IsNullOrEmpty(query.Service))
        //{
        //    sql.AppendLine($" and {StorageConstaaa.Current.ServiceName}=@serviceName");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "serviceName", Value = query.Service });
        //}

        if (!string.IsNullOrEmpty(query.ExType))
        {
            query.IsInstrument = true;
            sql.AppendLine($" and {StorageConst.Current.ExceptionType}=@exType");
            parameters.Add(new ClickHouseParameter { ParameterName = "exType", Value = query.ExType });
        }

        if (!string.IsNullOrEmpty(query.TraceId))
        {
            query.IsInstrument = true;
            sql.AppendLine($" and {StorageConst.Current.TraceId}=@traceId");
            parameters.Add(new ClickHouseParameter { ParameterName = "traceId", Value = query.TraceId });
        }

        if (!string.IsNullOrEmpty(query.TextField) && !string.IsNullOrEmpty(query.TextValue))
        {
            query.IsInstrument = true;
            if (string.Equals(query.TextField, StorageConst.Current.TraceId))
            {
                sql.AppendLine($" and {StorageConst.Current.TraceId}=@traceId");
                parameters.Add(new ClickHouseParameter { ParameterName = "traceId", Value = query.TextValue.Trim() });
            }
            else if (string.Equals(query.TextField, StorageConst.Current.SpanId))
            {
                sql.AppendLine($" and {StorageConst.Current.SpanId}=@spanId");
                parameters.Add(new ClickHouseParameter { ParameterName = "spanId", Value = query.TextValue.Trim() });
            }
            else
            {
                sql.AppendLine($" and `{query.TextField}` like @text");
                parameters.Add(new ClickHouseParameter { ParameterName = "text", Value = $"%{query.TextValue}%" });
            }
        }

        if (!string.IsNullOrEmpty(query.ExMessage))
        {
            query.IsInstrument = true;
            sql.AppendLine($" and {StorageConst.Current.ExceptionMessage} like @exMessage");
            parameters.Add(new ClickHouseParameter { ParameterName = "exMessage", Value = $"{query.ExMessage}%" });
        }

        //if (!string.IsNullOrEmpty(query.UrlParam))
        //{
        //    sql.AppendLine($" and {StorageConstaaa.Current.Trace.URLFull} like @urlParam");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "urlParam", Value = query.UrlParam });
        //}

        //if (!string.IsNullOrEmpty(query.BodyParam))
        //{
        //    sql.AppendLine($" and {StorageConstaaa.Current.Trace.HttpRequestBody} like @bodyParam");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "bodyParam", Value = query.BodyParam });
        //}

        //if (!string.IsNullOrEmpty(query.Body))
        //{
        //    sql.AppendLine($" and {StorageConstaaa.Current.Log.Body} like @body");
        //    parameters.Add(new ClickHouseParameter { ParameterName = "body", Value = query.Body });
        //}

        AppendEndpoint(query as ApmEndpointRequestDto, sql, parameters);
        AppendDuration(query as ApmTraceLatencyRequestDto, sql, parameters);
        if (!string.IsNullOrEmpty(query.Queries))
        {
            sql.AppendLine(!query.Queries.Trim().StartsWith("and ", StringComparison.CurrentCultureIgnoreCase) ? $" and {query.Queries}" : query.Queries);
        }

        return (sql.ToString(), ors, parameters);
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

    private static void AppendEndpoint(ApmEndpointRequestDto? traceQuery, StringBuilder sql, List<ClickHouseParameter> parameters, bool isMetric = false)
    {
        if (traceQuery == null)
            return;
        var name = "endpoint";
        if (traceQuery.IsLog.HasValue && traceQuery.IsLog.Value)
        {
            if (!string.IsNullOrEmpty(traceQuery.Endpoint))
            {
                sql.AppendLine($" and {ClickhouseHelper.GetName(StorageConst.Current.Log.Url, true)} like @{name}");
                parameters.Add(new ClickHouseParameter { ParameterName = name, Value = $"{traceQuery.Endpoint}%" });
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(traceQuery.Endpoint))
            {
                sql.AppendLine($" and {StorageConst.Current.Trace.URL}=@{name}");
                parameters.Add(new ClickHouseParameter { ParameterName = name, Value = traceQuery.Endpoint });
            }

            if (!string.IsNullOrEmpty(traceQuery.Method))
            {
                sql.AppendLine($" and {StorageConst.Current.Trace.HttpMethod}=@method");
                parameters.Add(new ClickHouseParameter { ParameterName = "method", Value = traceQuery.Method });
            }

            if (!isMetric)
            {
                if (traceQuery.IsTrace.HasValue && traceQuery.IsTrace!.Value && !string.IsNullOrEmpty(traceQuery.StatusCode))
                {
                    traceQuery.IsInstrument = true;
                    sql.AppendLine($" and {StorageConst.Current.Trace.HttpStatusCode}=@status_code");
                    parameters.Add(new ClickHouseParameter { ParameterName = "status_code", Value = traceQuery.StatusCode });
                }
            }
        }
    }

    private static void AppendDuration(ApmTraceLatencyRequestDto? query, StringBuilder sql, List<ClickHouseParameter> parameters)
    {
        if (query == null || !query.LatMin.HasValue && !query.LatMax.HasValue || query.IsMetric != null && query.IsMetric.Value) return;
        if (query.LatMin.HasValue && query.LatMin > 0)
        {
            sql.AppendLine($" and {StorageConst.Current.Trace.Duration} >=@minDuration");
            parameters.Add(new ClickHouseParameter { ParameterName = "minDuration", Value = (long)(query.LatMin * MILLSECOND) });
        }
        if (query.LatMax.HasValue && query.LatMax > 0)
        {
            sql.AppendLine($" and {StorageConst.Current.Trace.Duration} <=@maxDuration");
            parameters.Add(new ClickHouseParameter { ParameterName = "maxDuration", Value = (long)(query.LatMax * MILLSECOND) });
        }
    }

    private Task<DbDataReader> Query(string sql, IEnumerable<ClickHouseParameter> parameters)
    {
        var start = DateTime.Now;
        try
        {
            command.CommandText = sql;
            SetParameters(parameters);
            return command.ExecuteReaderAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "execute sql error:{Sqlraw}", sql);
            throw;
        }
        finally
        {
            var end = DateTime.Now;
            Log(start, end, sql, parameters);
        }
    }

    private Task<object?> Scalar(string sql, IEnumerable<ClickHouseParameter> parameters)
    {
        command.CommandText = sql;
        SetParameters(parameters);
        var start = DateTime.Now;
        try
        {
            return command.ExecuteScalarAsync();
        }
        finally
        {
            var end = DateTime.Now;
            Log(start, end, sql, parameters);
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


    private async Task<List<ChartLineCountDto>> GetChartCountData(string sql, IEnumerable<ClickHouseParameter> parameters, ComparisonTypes? comparisonTypes = null)
    {
        var result = new List<ChartLineCountDto>();
        //lock (lockObj)
        {

            using var currentReader = await Query(sql, parameters);
            await SetChartCountData(result, currentReader);
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
                using var previousReader = await Query(sql, parameters);
                await SetChartCountData(result, previousReader, true);

            }
        }

        return result;
    }

    private async Task SetChartCountData(List<ChartLineCountDto> result, DbDataReader reader, bool isPrevious = false)
    {
        var start = DateTime.Now;
        ChartLineCountDto? current = null;
        var list = new List<DateTime>();
        while (await reader.NextResultAsync())
            while (await reader.ReadAsync())
            {
                list.Add(DateTime.Now);
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
                list.Add(DateTime.Now);
            }
        var end = DateTime.Now;
        Log(start, end, default, default, true);
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








    private static (string, List<ClickHouseParameter>) GetMetricWhere<TRequest>(TRequest query, bool isContainsEndpoint = false) where TRequest : BaseApmRequestDto
    {
        List<ClickHouseParameter> parameters = new();
        var sql = new StringBuilder();
        sql.AppendLine($" {StorageConst.Current.Timestimap} between @startTime and @endTime");
        parameters.Add(new ClickHouseParameter { ParameterName = "startTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime });
        parameters.Add(new ClickHouseParameter { ParameterName = "endTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime });
        if (!string.IsNullOrEmpty(query.Env))
        {
            sql.AppendLine($" and {StorageConst.Current.Environment}=@environment");
            parameters.Add(new ClickHouseParameter { ParameterName = "environment", Value = query.Env });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.AppendLine($" and {StorageConst.Current.ServiceName}=@serviceName");
            parameters.Add(new ClickHouseParameter { ParameterName = "serviceName", Value = query.Service });
        }
        if (isContainsEndpoint)
            AppendEndpoint(query as ApmEndpointRequestDto, sql, parameters, true);
        return (sql.ToString(), parameters);
    }
}
