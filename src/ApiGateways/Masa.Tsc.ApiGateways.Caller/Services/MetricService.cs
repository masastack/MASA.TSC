// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class MetricService : BaseService
{
    static JsonSerializerOptions options;

    public MetricService(ICaller caller) : base(caller, "/api/metric")
    {
        if (options == null)
        {
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new QueryResultDataResponseConverter());
        }
    }

    public async Task<List<string>> GetNamesAsync()
    {
        return (await Caller.GetAsync<List<string>>($"{RootPath}/names"))!;
    }

    public async Task<Dictionary<string, Dictionary<string, List<string>>>> GetLabelValuesAsync(RequestLabelValuesDto param)
    {
        return (await Caller.GetByBodyAsync<Dictionary<string, Dictionary<string, List<string>>>>($"{RootPath}/label-values", param))!;
    }

    public async Task<string> GetRangeValuesAsync(RequestMetricAggDto param)
    {
        return (await Caller.GetByBodyAsync<string>($"{RootPath}/range-values", param))!;
    }

    public async Task<QueryResultDataResponse> GetQueryAsync(string query, DateTime time)
    {
        var result = (await Caller.GetAsync<QueryResultDataResponse>($"{RootPath}/query", new { query, time }))!;
        return ConvertResult(result);
    }

    public async Task<QueryResultDataResponse> GetQueryRangeAsync(RequestMetricAggDto param)
    {
        var result = (await Caller.GetByBodyAsync<QueryResultDataResponse>($"{RootPath}/query-range", param))!;
        return ConvertResult(result);
    }

    public async Task<List<QueryResultDataResponse>> GetMultiRangeAsync(RequestMultiQueryRangeDto param)
    {
        var result = (await Caller.GetByBodyAsync<List<QueryResultDataResponse>>($"{RootPath}/multi-range", param))!;
        if (result != null && result.Any())
        {
            return result.Select(item => ConvertResult(item)).ToList();
        }
        return default!;
    }

    public async Task<List<QueryResultDataResponse>> GetMultiQueryAsync(RequestMultiQueryDto param)
    {
        var result = (await Caller.GetByBodyAsync<List<QueryResultDataResponse>>($"{RootPath}/multi-query", param))!;
        if (result != null && result.Any())
        {
            return result.Select(item => ConvertResult(item)).ToList();
        }
        return default!;
    }

    public async Task<List<string>> GetValues(RequestMetricListDto param)
    {
        return (await Caller.GetAsync<List<string>>($"{RootPath}/values", param))!;
    }

    private static QueryResultDataResponse ConvertResult(QueryResultDataResponse result)
    {
        return JsonSerializer.Deserialize<QueryResultDataResponse>(JsonSerializer.Serialize(result), options)!;
    }
}