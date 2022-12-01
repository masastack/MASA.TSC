// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public class QueryHandler
{
    private readonly ITraceService _traceService;

    public QueryHandler(ITraceService traceService)
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
    public async Task GetListAsync(TraceListQuery query)
    {
        var queryDto = new BaseRequestDto
        {
            End = query.End,
            //Endpoint = query.Endpoint,
            Instance = query.Instance,
            Page = query.Page,
            Service = query.Service,
            PageSize = query.Size,
            Start = query.Start,
            TraceId = query.TraceId,
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = false }
        };
        if (!string.IsNullOrEmpty(query.Endpoint))
        {
            var tttt = new FieldConditionDto
            {
                Name = "Attributes.http.url",
                Type = ConditionTypes.Equal,
                Value = query.Endpoint
            };

            if (queryDto.Conditions == null)
                queryDto.Conditions = new FieldConditionDto[] {
                    tttt
                };
            else
            {
                var list = queryDto.Conditions.ToList();
                list.Add(tttt);
                queryDto.Conditions=list;
            }
        }

        query.Result = await _traceService.ListAsync(queryDto);
        if (query.Result == null)
            query.Result = new PaginatedListBase<TraceResponseDto>();
    }

    [EventHandler]
    public async Task GetAttrValuesAsync(TraceAttrValuesQuery query)
    {
        if (string.IsNullOrEmpty(query.Data.Service))
        {
            query.Data.Name = ElasticConstant.ServiceName;
        }
        else if (query.Data.Instance == null)
        {
            query.Data.Name = ElasticConstant.ServiceInstance;
        }
        else
        {
            query.Data.Name = "Attributes.http.url";// ElasticConstant.Endpoint;
        }

        query.Result = (IEnumerable<string>)await _traceService.AggregateAsync(query.Data);
        if (query.Result == null)
            query.Result = Array.Empty<string>();
    }

    [EventHandler]
    public async Task AggregateAsync(TraceAggregationQuery query)
    {
        var data = await _traceService.AggregateAsync(query.Data);
        query.Result = data;
    }
}