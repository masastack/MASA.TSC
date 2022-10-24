// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class MetricService : ServiceBase
{
    public MetricService() : base("/api/metric")
    {
        App.MapGet($"{BaseUri}/names", GetNamesAsync);
        App.MapGet($"{BaseUri}/label-values", GetLabelValuesAsync);
        App.MapGet($"{BaseUri}/range-values", GetRangeValuesAsync);
        App.MapGet($"{BaseUri}/query", GetQueryAsync);
        App.MapGet($"{BaseUri}/query-range", GetQueryRangeAsync);
    }

    private async Task<IEnumerable<string>> GetNamesAsync([FromServices] IEventBus eventBus, [FromQuery] string? match)
    {
        var query = new MetricQuery(match?.Split(',') ?? default!);
        await eventBus.PublishAsync(query);
        return query.Result ?? Array.Empty<string>();
    }

    private async Task<Dictionary<string, Dictionary<string, List<string>>>> GetLabelValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestLabelValuesDto param)
    {
        var query = new LableValuesQuery(param.Match, param.Start, param.End);
        await eventBus.PublishAsync(query);
        return query.Result ?? new Dictionary<string, Dictionary<string, List<string>>>();
    }

    private async Task<string> GetRangeValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestMetricAggDto param)
    {
        var query = new RangeValueQuery(param.Match, param.Start, param.End);

        await eventBus.PublishAsync(query);
        return query.Result ?? string.Empty;
    }

    private async Task<QueryResultDataResponse> GetQueryAsync([FromServices] IEventBus eventBus, [FromQuery] string query, [FromQuery] DateTime time)
    {
        var result = new InstantQuery(query, time);
        await eventBus.PublishAsync(result);
        return result.Result;
    }

    private async Task<QueryResultDataResponse> GetQueryRangeAsync([FromServices] IEventBus eventBus, [FromBody] RequestMetricAggDto param)
    {
        var query = new RangeQuery(param.Match, param.Step, param.Start, param.End);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
