// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class LogService : ServiceBase
{
    public LogService(IServiceCollection services) : base(services, "/api/log")
    {
        App.MapGet($"{BaseUri}/agg", AggegationAsync);
        App.MapGet($"{BaseUri}/latest", GetLatestAsync);
    }

    private async Task<IEnumerable<KeyValuePair<string, string>>> AggegationAsync([FromServices] IEventBus eventBus, [FromBody]RequestLogAggDto param)
    {
       var query = new LogAggQuery
        {
            Start = param.Start,
            End = param.End,
            Query = param.Query,
            FieldMaps = param.FieldMaps
        };
        await eventBus.PublishAsync(query);
        return query.Result;
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
}