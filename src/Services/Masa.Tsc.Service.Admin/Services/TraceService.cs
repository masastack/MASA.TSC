// Copyright (c) MASA Stack All rights reserved.
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
        App.MapGet($"{BaseUri}/getTraceIdByMetric", GetTraceIdByMetricAsync);
    }

    private async Task<IEnumerable<TraceResponseDto>> GetAsync([FromServices] IEventBus eventBus, [FromRoute] string traceId)
    {
        var query = new TraceDetailQuery(traceId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<string> GetTraceIdByMetricAsync([FromServices] IEventBus eventBus, string service, string url, DateTime start, DateTime end)
    {
        var query = new TraceIdByMetricQuery(service, url, start, end);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginatedListBase<TraceResponseDto>> GetListAsync([FromServices] IEventBus eventBus, RequestTraceListDto model)
    {
        var query = new TraceListQuery(model.Service, model.Instance, model.Endpoint, model.TraceId, model.Start, model.End, model.Page, model.PageSize, model.IsDesc, model.Keyword, model.IsError);
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

    public async Task<IEnumerable<TraceResponseDto>> GetNextAsync([FromServices] IEventBus eventBus, string service, string traceId, DateTime time, string url, bool isNext)
    {
        var query = new TraceDetailNextQuery(service, traceId, time, url, isNext);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
