// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public class QueryHandler
{
    private readonly ITraceService _traceService;
    private readonly IMasaStackConfig _masaStackConfig;
    private readonly IWebHostEnvironment _environment;
    private readonly IMultiEnvironmentContext _multiEnvironment;
    private readonly IMasaConfiguration _masaConfiguration;

    public QueryHandler(ITraceService traceService, IMasaConfiguration masaConfiguration, IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IMultiEnvironmentContext multiEnvironment)
    {
        _traceService = traceService;
        _masaConfiguration = masaConfiguration;
        _masaStackConfig = masaStackConfig;
        _environment = environment;
        _multiEnvironment = multiEnvironment;
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
        var partten = @"\{([^\}/]+)\}";
        Regex.Replace(query.Url, partten, "*");

        var env = _masaStackConfig.GetServiceEnvironmentName(_environment, query.Service, _multiEnvironment.CurrentEnvironment);
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
        queryDto.SetEnv(_masaStackConfig.GetServiceEnvironmentName(_environment, query.Service, _multiEnvironment.CurrentEnvironment));
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
            var errorPorts = _masaConfiguration.GetTraceErrorStatus(_masaStackConfig);
            list.Add(new FieldConditionDto
            {
                Name = ElasticSearchConst.HttpPort,
                Type = ConditionTypes.In,
                Value = errorPorts.Select(num => (object)num)
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

        query.Data.SetEnv(_masaStackConfig.GetServiceEnvironmentName(_environment, query.Data.Service, _multiEnvironment.CurrentEnvironment));
        query.Result = (IEnumerable<string>)await _traceService.AggregateAsync(query.Data);
        if (query.Result == null)
            query.Result = Array.Empty<string>();
    }

    [EventHandler]
    public async Task AggregateAsync(TraceAggregationQuery query)
    {
        query.Data.SetEnv(_masaStackConfig.GetServiceEnvironmentName(_environment, query.Data.Service, _multiEnvironment.CurrentEnvironment));
        var data = await _traceService.AggregateAsync(query.Data);
        query.Result = data;
    }
}