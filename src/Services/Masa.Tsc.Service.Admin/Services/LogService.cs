// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class LogService : ServiceBase
{
    public LogService(IServiceCollection services) : base(services, "/api/log")
    {
        App.MapGet($"{BaseUri}/aggregate", AggregateAsync);
        App.MapGet($"{BaseUri}/latest", GetLatestAsync);
        App.MapGet($"{BaseUri}/mapping", GetMappingFieldAsync);
        App.MapGet($"{BaseUri}/list", GetPageAsync);
    }

    private async Task<IEnumerable<KeyValuePair<string, string>>> AggregateAsync([FromServices] IEventBus eventBus, [FromBody] RequestAggregationDto param)
    {
        var query = new LogAggQuery(param.FieldMaps, param.RawQuery, param.Start, param.End, param.Interval);        
        await eventBus.PublishAsync(query);
        return query.Result ?? Array.Empty<KeyValuePair<string, string>>();
    }

    private async Task<object> GetLatestAsync([FromServices] IEventBus eventBus, [FromBody] RequestLogLatestDto param)
    {
        var query = new LatestLogQuery
        {
            Start = param.Start,
            End = param.End,
            Query = param.Query,
            IsDesc = param.IsDesc
        };
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<Contracts.Admin.MappingResponse>> GetMappingFieldAsync([FromServices] IEventBus eventBus)
    {
        var query = new LogFieldQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<object>> GetPageAsync([FromServices] IEventBus eventBus, LogPageQueryDto param)
    {
        var query = new LogsQuery(param.Query, param.Start, param.End, param.Page, param.PageSize, param.Sorting);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}