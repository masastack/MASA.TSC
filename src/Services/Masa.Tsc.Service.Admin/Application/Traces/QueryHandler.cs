// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public class QueryHandler : EnvQueryHandler
{
    private readonly ITraceService _traceService;

    public QueryHandler(ITraceService traceService, IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IMultiEnvironmentContext multiEnvironment)
        : base(masaStackConfig, environment, multiEnvironment)
    {
        _traceService = traceService;
    }

    [EventHandler]
    public async Task GetDetailAsync(TraceDetailQuery query)
    {
        query.Result = await _traceService.GetAsync(query.TraceId);
        if (query.Result == null)
            query.Result = Array.Empty<TraceResponseDto>();
    }

    [EventHandler]
    public async Task GetTraceIdByMetricAsync(TraceIdByMetricQuery query)
    {
        ArgumentNullException.ThrowIfNull(query.Service);
        ArgumentNullException.ThrowIfNull(query.Url);
        var env = GetServiceEnvironmentName(query.Service);
        var traceId = await _traceService.GetElasticClient().GetByMetricAsync(query.Service, query.Url, query.Start, query.End, env);
        query.Result = traceId ?? string.Empty;
    }

    [EventHandler]
    public async Task GetListAsync(TraceListQuery query)
    {
        var queryDto = new BaseRequestDto
        {
            End = query.End,
            Instance = query.Instance,
            Page = query.Page,
            Service = query.Service,
            PageSize = query.Size,
            Start = query.Start,
            TraceId = query.TraceId,
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = query.IsDesc }
        };
        queryDto.SetEnv(GetServiceEnvironmentName(query.Service));
        var list = queryDto.Conditions?.ToList() ?? new();
        if (!string.IsNullOrEmpty(query.Endpoint))
        {
            var endpointCondition = new FieldConditionDto
            {
                Name = ElasticSearchConst.URL,
                Type = ConditionTypes.Equal,
                Value = query.Endpoint
            };
            list.Add(endpointCondition);
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            if (query.Keyword.IndexOf('}') >= 0)
                queryDto.RawQuery = query.Keyword;
            else
                queryDto.Keyword = query.Keyword;
        }

        if (query.IsError)
        {
            list.Add(new FieldConditionDto
            {
                Name = ElasticSearchConst.HttpPort,
                Type = ConditionTypes.In,
                Value = ConfigConst.TraceErrorStatus.Select(num => (object)num)
            });
        }
        queryDto.Conditions = list;

        query.Result = await _traceService.ListAsync(queryDto);
        if (query.Result == null)
            query.Result = new PaginatedListBase<TraceResponseDto>();
    }

    [EventHandler]
    public async Task GetAttrValuesAsync(TraceAttrValuesQuery query)
    {
        var list = query.Data.Conditions?.ToList() ?? new();

        list.Add(new FieldConditionDto
        {
            Name = "Kind",
            Type = ConditionTypes.Equal,
            Value = "SPAN_KIND_SERVER"
        });

        if (string.IsNullOrEmpty(query.Data.Service))
        {
            query.Data.Name = ElasticConstant.ServiceName;
            if (!string.IsNullOrEmpty(query.Data.Keyword))
            {
                list.Add(new FieldConditionDto
                {
                    Name = ElasticConstant.ServiceName,
                    Type = ConditionTypes.Regex,
                    Value = $"*{query.Data.Keyword}*"
                });
            }
        }
        else if (query.Data.Instance == null)
        {
            query.Data.Name = ElasticConstant.ServiceInstance;
            if (!string.IsNullOrEmpty(query.Data.Keyword))
            {
                list.Add(new FieldConditionDto
                {
                    Name = ElasticConstant.ServiceInstance,
                    Type = ConditionTypes.Regex,
                    Value = $"*{query.Data.Keyword}*"
                });
            }
        }
        else
        {
            query.Data.Name = "Attributes.http.target";
            if (!string.IsNullOrEmpty(query.Data.Keyword))
            {
                list.Add(new FieldConditionDto
                {
                    Name = ElasticConstant.Endpoint,
                    Type = ConditionTypes.Regex,
                    Value = $"*{query.Data.Keyword}*"
                });
            }
        }
        query.Data.Conditions = list;
        query.Data.Keyword = default!;

        query.Data.SetEnv(GetServiceEnvironmentName(query.Data.Service));
        query.Result = (IEnumerable<string>)await _traceService.AggregateAsync(query.Data);
        if (query.Result == null)
            query.Result = Array.Empty<string>();
    }

    [EventHandler]
    public async Task AggregateAsync(TraceAggregationQuery query)
    {
        query.Data.SetEnv(GetServiceEnvironmentName(query.Data.Service));
        var data = await _traceService.AggregateAsync(query.Data);
        query.Result = data;
    }

    [EventHandler]
    public async Task GetNextPrevAsync(TraceDetailNextQuery query)
    {
        var queryDto = new SimpleAggregateRequestDto
        {
            Service = query.Service,
            Sort = new FieldOrderDto
            {
                Name = ElasticConstant.Trace.Timestamp,
                IsDesc = !query.IsNext
            },
            Page = 1,
            PageSize = 1
        };

        var list = new List<FieldConditionDto>() {
            new FieldConditionDto{
                Name=ElasticSearchConst.URL,
                Type =ConditionTypes.Equal,
                Value=query.Url
            },
            new FieldConditionDto{
                Name=ElasticConstant.TraceId,
                Type =ConditionTypes.NotEqual,
                Value=query.TraceId
            }
        };

        if (query.IsNext)
        {
            list.Add(new FieldConditionDto
            {
                Name = ElasticConstant.Trace.Timestamp,
                Type = ConditionTypes.Great,
                Value = query.Time
            });
        }
        else
        {
            list.Add(new FieldConditionDto
            {
                Name = ElasticConstant.Trace.Timestamp,
                Type = ConditionTypes.Less,
                Value = query.Time
            });
        }
        queryDto.Conditions = list;
        var env = GetServiceEnvironmentName(query.Service);
        queryDto.SetEnv(env);
        var data = await _traceService.ListAsync(queryDto);
        if (data.Result == null || !data.Result.Any())
        {
            query.Result = Array.Empty<TraceResponseDto>();
            return;
        }

        var traceId = data.Result.First().TraceId;
        var detailQuery = new TraceDetailQuery(traceId);
        await GetDetailAsync(detailQuery);
        query.Result = detailQuery.Result;
    }
}