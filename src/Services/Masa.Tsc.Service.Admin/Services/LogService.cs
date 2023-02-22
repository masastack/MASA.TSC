// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class LogService : ServiceBase
{
    public LogService() : base("/api/log")
    {
        App.MapGet($"{BaseUri}/aggregate", AggregateAsync);
        App.MapGet($"{BaseUri}/latest", GetLatestAsync);
        App.MapGet($"{BaseUri}/mapping", GetMappingFieldAsync);
        App.MapGet($"{BaseUri}/list", GetPageAsync);
    }

    private async Task<object> AggregateAsync([FromServices] IEventBus eventBus, [FromBody] SimpleAggregateRequestDto param)
    {
        var query = new LogAggQuery(param);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<LogResponseDto> GetLatestAsync([FromServices] IEventBus eventBus, [FromBody] RequestLogLatestDto param)
    {
        var query = new LatestLogQuery(param.Start, param.End, param.Query, param.IsDesc);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<IEnumerable<MappingResponseDto>> GetMappingFieldAsync([FromServices] IEventBus eventBus)
    {
        var query = new LogFieldQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginatedListBase<LogResponseDto>> GetPageAsync([FromServices] IEventBus eventBus, LogPageQueryDto param)
    {
        var query = new LogsQuery(param.Query, param.Start, param.End, param.Page, param.PageSize, param.IsDesc, JobTaskId: param.TaskId,SpanId:param.SpanId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}