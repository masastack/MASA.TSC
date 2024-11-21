// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class MetricService : ServiceBase
{
    public MetricService() : base("/api/metric")
    {
        App.MapGet($"{BaseUri}/label-values", GetLabelValuesAsync);
        App.MapGet($"{BaseUri}/range-values", GetRangeValuesAsync);
        App.MapGet($"{BaseUri}/query-range", GetQueryRangeAsync);
        App.MapGet($"{BaseUri}/multi-range", GetMultiRangeAsync);
        App.MapGet($"{BaseUri}/multi-query", GetMultiQueryAsync);
    }

    public async Task<IEnumerable<string>> GetNamesAsync([FromServices] IEventBus eventBus, [FromQuery] string? match)
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

    public async Task<QueryResultDataResponse> GetQueryAsync([FromServices] IEventBus eventBus, [FromQuery] string query, [FromQuery] string time)
    {
        var result = new InstantQuery(query, time.ParseUTCTime());
        await eventBus.PublishAsync(result);
        return result.Result;
    }

    private async Task<QueryResultDataResponse> GetQueryRangeAsync([FromServices] IEventBus eventBus, [FromBody] RequestMetricAggDto param)
    {
        var query = new RangeQuery(param.Match, param.Step, param.Start, param.End);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<QueryResultDataResponse>> GetMultiRangeAsync([FromServices] IEventBus eventBus, [FromBody] RequestMultiQueryRangeDto param)
    {
        var query = new MultiRangeQuery(param);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<QueryResultDataResponse>> GetMultiQueryAsync([FromServices] IEventBus eventBus, [FromBody] RequestMultiQueryDto param)
    {
        var query = new MultiQuery(param);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<string>> GetValuesAsync([FromServices] IEventBus eventBus, string? layer, string? service, string? instance, string? endpoint, MetricValueTypes type)
    {
        var query = new ValuesQuery(layer!, service!, instance!, endpoint!, type);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}