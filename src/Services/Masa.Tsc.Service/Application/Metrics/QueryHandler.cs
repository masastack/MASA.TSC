// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public class QueryHandler
{
    private readonly IMasaPromethusClient _promethusClient;
    private readonly ILogger _logger;

    public QueryHandler(IMasaPromethusClient masaPromethusClient, ILogger<QueryHandler> logger)
    {
        _promethusClient = masaPromethusClient;
        _logger = logger;
    }

    [EventHandler]
    public async Task GetRangeAggregationAsync(RangeQuery query)
    {
        var data = await _promethusClient.QueryRangeAsync(new QueryRangeRequest
        {
            End = query.End.ToString(),
            Start = query.Start.ToString(),
            Query = query.Match,
            Step = ((int)Math.Ceiling((query.End - query.Start).TotalSeconds)).ToString()
        });

        if (data.Status == ResultStatuses.Success)
        {
            if (data.Data == null || data.Data.Result == null || !data.Data.Result.Any())
                return;

            query.Result = (string)data.Data.Result[1];
            return;
        }

        _logger.LogError("request failed {data.ErrorType} {data.Error}", data);
    }

    [EventHandler]
    public async Task GetMetricsAsync(MetricQuery query)
    {
        var data = await _promethusClient.LabelValuesQueryAsync(new LableValueQueryRequest
        {
            Match = query.Match
        });

        if (data.Status == ResultStatuses.Success)
        {
            if (data.Data == null || data.Data == null || !data.Data.Any())
                return;

            query.Result = data.Data;
            return;
        }

        _logger.LogError("request failed {data.ErrorType} {data.Error}", data);
    }

    [EventHandler]
    public async Task GetLableValuesAsync(LableValuesQuery query)
    {
        var data = await _promethusClient.SeriesAsync(new MetaDataQueryRequest
        {
            Match = query.Match,
            End = query.End.ToString(),
            Start = query.Start.ToString()
        });

        if (data.Status == ResultStatuses.Success)
        {
            if (data.Data == null || data.Data == null || !data.Data.Any())
                return;

            query.Result = ConverToKeyValues(data.Data);
            return;
        }

        _logger.LogError("request failed {data.ErrorType} {data.Error}", data);
    }

    private Dictionary<string, Dictionary<string, List<string>>> ConverToKeyValues(IEnumerable<IDictionary<string, string>> sources)
    {
        var result = new Dictionary<string, Dictionary<string, List<string>>>();
        string matchKey = "__name__";
        foreach (var item in sources)
        {
            Dictionary<string, List<string>> dic;
            if (item.ContainsKey(matchKey))
            {
                if (result.ContainsKey(item[matchKey]))
                {
                    dic = result[item[matchKey]];
                }
                else
                {
                    dic = new Dictionary<string, List<string>>();
                    result.Add(item[matchKey], dic);
                }
            }
            else
            {
                continue;
            }

            foreach (var key in item.Keys)
            {
                if (string.Equals(key, matchKey, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                else
                {
                    if (dic.ContainsKey(key))
                    {
                        if (!dic[key].Contains(item[key]))
                            dic[key].Add(item[key]);
                    }
                    else
                    {
                        dic.Add(item[key], new List<string>() { item[key] });
                    }
                }
            }
        }

        return result;
    }
}
