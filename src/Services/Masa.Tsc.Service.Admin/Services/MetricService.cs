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
        if (param.Labels != null && param.Labels.Any())
        {
            param.Match = $"{param.Match}{{{string.Join(',', param.Labels)}}}";
        }

        var query = new RangeQuery(param.Match, Step: string.Empty, param.Start, param.End);

        await eventBus.PublishAsync(query);
        return query.Result ?? string.Empty;
    }
}
