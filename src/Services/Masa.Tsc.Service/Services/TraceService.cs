// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class TraceService : ServiceBase
{
    public TraceService(IServiceCollection services) : base(services, "/api/trace")
    {
        App.MapGet($"{BaseUri}/{{traceId}}", GetAsync);
        App.MapGet($"{BaseUri}/list", GetListAsync);
        App.MapGet($"{BaseUri}/attr-values", GetAttrValuesAsync);
        App.MapGet($"{BaseUri}/aggregation", GetAttrValuesAsync);
    }

    private async Task<IEnumerable<object>> GetAsync([FromServices] IEventBus eventBus, [FromRoute] string traceId)
    {
        var query = new TraceDetailQuery(traceId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<object>> GetListAsync([FromServices] IEventBus eventBus, RequestTraceListDto model)
    {
        var query = new TraceListQuery(model.Service, model.Instance, model.Endpoint, model.TraceId, model.Start, model.End, model.Page, model.PageSize);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<string>> GetAttrValuesAsync([FromServices] IEventBus eventBus, RequestAttrDataDto model)
    {
        var query = new TraceAttrValuesQuery(model.Query, model.Name, model.Keyword, model.Start, model.End, model.Max);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
