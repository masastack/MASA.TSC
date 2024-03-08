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
        var dto = new BaseRequestDto
        {
            Service = query.Service,
            Start = query.Start,
            End = query.End,
            Conditions = new List<FieldConditionDto> { new FieldConditionDto {
             Name=StorageConst.URL,
             Type= ConditionTypes.Equal,
                Value=query.Url
            } }
        };
        dto.SetEnv(env);
        var traceId = await _traceService.GetMaxDelayTraceIdAsync(dto);
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

        var list = queryDto.Conditions?.ToList() ?? new();
        if (!string.IsNullOrEmpty(query.Endpoint))
        {
            var endpointCondition = new FieldConditionDto
            {
                Name = StorageConst.URL,
                Type = ConditionTypes.Equal,
                Value = query.Endpoint
            };
            list.Add(endpointCondition);
        }
        bool isRawQuery = query.Keyword.IsRawQuery(ConfigConst.StorageConst.IsElasticSearch, ConfigConst.StorageConst.IsClickhouse);
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            if (isRawQuery)
            {
                isRawQuery = true;
                queryDto.RawQuery = query.Keyword;
            }
            else
            {
                queryDto.Keyword = query.Keyword;
            }
        }

        if (!string.IsNullOrEmpty(query.SpanId))
        {
            list.Add(new FieldConditionDto
            {
                Name = StorageConst.SpanId,
                Type = ConditionTypes.Equal,
                Value = query.SpanId
            });
        }

        if (query.IsError)
        {
            list.Add(new FieldConditionDto
            {
                Name = StorageConst.HttpPort,
                Type = ConditionTypes.In,
                Value = ConfigConst.TraceErrorStatus.Select(num => (object)num)
            });
        }


        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(query.Env))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Resource.service.namespace",
                Type = ConditionTypes.Equal,
                Value = query.Env
            });
        }
        else
        {
            if (string.IsNullOrEmpty(query.TraceId) && !query.IsError && !isRawQuery)
                queryDto.SetEnv(GetServiceEnvironmentName(query.Service));
        }

        if (query.LatMin.HasValue && query.LatMin.Value >= 0)
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Duration",
                Type = ConditionTypes.GreatEqual,
                Value = query.LatMin.Value,
            });
        }

        if (query.LatMax.HasValue && query.LatMax.Value >= 0 && (
            !query.LatMin.HasValue
            || query.LatMin.HasValue && query.LatMax - query.LatMin.Value > 0))
            conditions.Add(new FieldConditionDto
            {
                Name = "Duration",
                Type = ConditionTypes.LessEqual,
                Value = query.LatMax.Value,
            });
        if (conditions.Any())
            queryDto.Conditions = conditions;

        queryDto.Conditions = list;

        query.Result = await _traceService.ListAsync(queryDto);
        query.Result ??= new PaginatedListBase<TraceResponseDto>();
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
            query.Data.Name = StorageConst.ServiceName;
        }
        else if (query.Data.Instance == null)
        {
            query.Data.Name = StorageConst.ServiceInstance;
        }
        else
        {
            query.Data.Name = "Attributes.http.target";
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
                Name = StorageConst.Timestimap(ConfigConst.StorageConst.IsElasticSearch, ConfigConst.StorageConst.IsClickhouse),
                IsDesc = !query.IsNext
            },
            Page = 1,
            PageSize = 1
        };

        var list = new List<FieldConditionDto>() {
            new FieldConditionDto{
                Name=StorageConst.URL,
                Type =ConditionTypes.Equal,
                Value=query.Url
            },
            new FieldConditionDto{
                Name=StorageConst.TraceId,
                Type =ConditionTypes.NotEqual,
                Value=query.TraceId
            }
        };

        if (query.IsNext)
        {
            list.Add(new FieldConditionDto
            {
                Name = StorageConst.Timestimap(ConfigConst.StorageConst.IsElasticSearch, ConfigConst.StorageConst.IsClickhouse),
                Type = ConditionTypes.Great,
                Value = query.Time
            });
        }
        else
        {
            list.Add(new FieldConditionDto
            {
                Name = StorageConst.Timestimap(ConfigConst.StorageConst.IsElasticSearch, ConfigConst.StorageConst.IsClickhouse),
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

        var traceId = data.Result[0].TraceId;
        var detailQuery = new TraceDetailQuery(traceId);
        await GetDetailAsync(detailQuery);
        query.Result = detailQuery.Result;
    }

    [EventHandler]
    public void GetErrorStatus(TraceErrorStatusQuery query)
    {
        query.Result = ConfigConst.TraceErrorStatus;
    }
}