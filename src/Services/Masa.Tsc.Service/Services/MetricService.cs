// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class MetricService : ServiceBase
{
    public MetricService(IServiceCollection services) : base(services, "/api/metric")
    {
        App.MapGet($"{BaseUri}/names", GetNamesAsync);
        App.MapGet($"{BaseUri}/label-values", GetLabelValuesAsync);
        App.MapGet($"{BaseUri}/range-values", GetRangeValuesAsync);
    }

    private async Task<IEnumerable<string>> GetNamesAsync([FromServices] IEventBus eventBus, [FromQuery] string? match)
    {
        var query = new MetricQuery() { Match = match?.Split(',') ?? default! };
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<Dictionary<string, Dictionary<string, List<string>>>> GetLabelValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestLabelValuesDto param)
    {
        var query = new LableValuesQuery
        {
            Start = param.Start,
            End = param.End,
            Match = param.Match
        };
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<string> GetRangeValuesAsync([FromServices] IEventBus eventBus, [FromBody] RequestMetricAggDto param)
    {
        var query = new RangeQuery()
        {
            Start = param.Start,
            End = param.End,
            Match = param.Match
        };
        if (param.Labels != null && param.Labels.Any())
        {
            query.Match = $"{param.Match}{{{string.Join(',', param.Labels)}}}";
        }

        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
