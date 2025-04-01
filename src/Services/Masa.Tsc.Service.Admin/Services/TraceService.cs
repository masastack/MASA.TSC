// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class TraceService : ServiceBase
{
    public TraceService() : base("/api/trace")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        App.MapGet($"{BaseUri}/{{traceId}}", GetAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/list", GetListAsync).RequireAuthorization();
        App.MapPost($"{BaseUri}/attr-values", GetAttrValuesAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/aggregate", AggregateAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/getTraceIdByMetric", GetTraceIdByMetricAsync).RequireAuthorization();
    }

    private async Task<IEnumerable<TraceResponseDto>> GetAsync([FromServices] IEventBus eventBus, [FromRoute] string traceId, [FromQuery] string start, [FromQuery] string end)
    {
        var query = new TraceDetailQuery(traceId, start, end);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<string> GetTraceIdByMetricAsync([FromServices] IEventBus eventBus, string service, string url, string start, string end)
    {
        var query = new TraceIdByMetricQuery(service, url, start.ParseUTCTime(), end.ParseUTCTime());
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginatedListBase<TraceResponseDto>> GetListAsync([FromServices] IEventBus eventBus, RequestTraceListDto model)
    {
        var query = new TraceListQuery(model.Service, model.Instance, model.Endpoint, model.TraceId, model.Start, model.End, model.Page, model.PageSize, model.IsDesc, model.Keyword, model.IsError, model.Env, model.LatMin, model.LatMax, model.SpanId, model.HasPage);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<string>> GetAttrValuesAsync([FromServices] IEventBus eventBus, [FromBody] SimpleAggregateRequestDto model)
    {
        var query = new TraceAttrValuesQuery(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<object> AggregateAsync([FromServices] IEventBus eventBus, [FromBody] SimpleAggregateRequestDto param)
    {
        var query = new TraceAggregationQuery(param);
        await eventBus.PublishAsync(query);
        return query.Result!;
    }

    public async Task<IEnumerable<TraceResponseDto>> GetNextAsync([FromServices] IEventBus eventBus, string service, string traceId, string time, string url, bool isNext)
    {
        var query = new TraceDetailNextQuery(service, traceId, time.ParseUTCTime(), url, isNext);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<int[]> GetErrorStatusAsync([FromServices] IEventBus eventBus)
    {
        var query = new TraceErrorStatusQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
