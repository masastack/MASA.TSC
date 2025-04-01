// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class LogService : ServiceBase
{
    public LogService() : base("/api/log")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        App.MapGet($"{BaseUri}/aggregate", AggregateAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/latest", GetLatestAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/mapping", GetMappingFieldAsync).RequireAuthorization();
        App.MapGet($"{BaseUri}/list", GetPageAsync).RequireAuthorization();
    }

    private async Task<object> AggregateAsync([FromServices] IEventBus eventBus, [FromBody] SimpleAggregateRequestDto param)
    {
        param.SetEnableExceptError();
        var query = new LogAggQuery(param);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<LogResponseDto> GetLatestAsync([FromServices] IEventBus eventBus, [FromBody] RequestLogLatestDto param)
    {        
        var query = new LatestLogQuery(param.Start, param.End, param.Service, param.Query, param.IsDesc);
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
        var query = new LogsQuery(param.Query, param.Start, param.End, param.Page, param.PageSize, param.IsDesc, param.SortField, JobTaskId: param.TaskId, SpanId: param.SpanId, Service: param.Service, LogLevel: param.LogLevel, Env: param.Env, IsLimitEnv: param.IsLimitEnv);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<LogErrorDto>> GetErrorTypesAsync([FromServices] IEventBus eventBus, string service, string start, string end)
    {
        var query = new LogErrorTypesQuery(service, start.ParseUTCTime(), end.ParseUTCTime());
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}