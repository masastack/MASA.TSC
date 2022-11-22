﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TraceService : ServiceBase
{
    public TraceService() : base("/api/trace")
    {
        App.MapGet($"{BaseUri}/{{traceId}}", GetAsync);
        App.MapGet($"{BaseUri}/list", GetListAsync);
        App.MapGet($"{BaseUri}/attr-values", GetAttrValuesAsync);
        App.MapGet($"{BaseUri}/aggregate", AggregateAsync);
    }

    private async Task<IEnumerable<TraceDto>> GetAsync([FromServices] IEventBus eventBus, [FromRoute] string traceId)
    {
        var query = new TraceDetailQuery(traceId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<TraceDto>> GetListAsync([FromServices] IEventBus eventBus, RequestTraceListDto model)
    {
        var query = new TraceListQuery(model.Service, model.Instance, model.Endpoint, model.TraceId, model.Start, model.End, model.Page, model.PageSize);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<string>> GetAttrValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestAttrDataDto model)
    {
        var query = new TraceAttrValuesQuery(model.Query, model.Name, model.Keyword, model.Start, model.End, model.Max);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ChartLineDataDto<ChartPointDto>> AggregateAsync([FromServices] IEventBus eventBus, [FromBody] RequestAggregationDto param)
    {
        var query = new TraceAggregationQuery(true, true, param.FieldMaps, param.Queries, param.Start, param.End, param.Interval);
        await eventBus.PublishAsync(query);
        return query.Result!;
    }
}
