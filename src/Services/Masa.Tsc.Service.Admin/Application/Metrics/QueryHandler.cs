// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public class QueryHandler
{
    private readonly IMasaPrometheusClient _prometheusClient;
    private readonly ILogger _logger;

    public QueryHandler(IMasaPrometheusClient masaPrometheusClient, ILogger<QueryHandler> logger)
    {
        _prometheusClient = masaPrometheusClient;
        _logger = logger;
    }

    [EventHandler]
    public async Task GetRangeValuesAsync(RangeValueQuery query)
    {
        var data = await _prometheusClient.QueryRangeAsync(new QueryRangeRequest
        {
            End = query.End.ToUnixTimestamp().ToString(),
            Start = query.Start.ToUnixTimestamp().ToString(),
            Query = query.Match,
            Step = ((int)Math.Ceiling((query.End - query.Start).TotalSeconds)).ToString()
        });

        if (data.Status == ResultStatuses.Success)
        {
            if (data.Data == null || data.Data.Result == null || !data.Data.Result.Any())
                return;

            switch (data.Data.ResultType)
            {
                case ResultTypes.Matrix:
                    {
                        var matrixData = (QueryResultMatrixRangeResponse[])data.Data.Result;
                        if (matrixData != null && matrixData[0] != null && matrixData[0].Values != null)
                        {
                            var first = matrixData[0].Values?.First();
                            if (first != null && first.Length > 1)
                                query.Result = (string)first[1];
                        }
                    }
                    break;
                case ResultTypes.Vector:
                    {
                        var instantData = (QueryResultInstantVectorResponse[])data.Data.Result;
                        if (instantData != null && instantData[0] != null)
                        {
                            var array = instantData[0].Value;
                            if (array != null && array.Length - 1 > 0)
                                query.Result = (string)array[1];
                        }
                    }
                    break;
                default:
                    {
                        query.Result = (string)data.Data.Result[1];
                    }
                    break;
            }
            return;
        }

        _logger.LogError("request failed {data.ErrorType} {data.Error}", data);
    }

    [EventHandler]
    public async Task GetQueryRangeAsync(RangeQuery query)
    {
        var data = await _prometheusClient.QueryRangeAsync(new QueryRangeRequest
        {
            End = query.End.ToUnixTimestamp().ToString(),
            Start = query.Start.ToUnixTimestamp().ToString(),
            Query = query.Match,
            Step = query.Step,
        });

        if (data.Status == ResultStatuses.Error)
            _logger.LogError("request failed {data.ErrorType} {data.Error}", data);

        query.Result = data.Data!;
    }

    [EventHandler]
    public async Task GetQueryAsync(InstantQuery query)
    {
        var data = await _prometheusClient.QueryAsync(new QueryRequest
        {
            Query = query.Match,
            Time = query.Time.ToUnixTimestamp().ToString()
        });
        query.Result = data.Data!;
        if (data.Status != ResultStatuses.Success)
        {
            _logger.LogError("request failed {data.ErrorType} {data.Error}", data);
        }
    }

    [EventHandler]
    public async Task GetMetricsAsync(MetricQuery query)
    {
        var data = await _prometheusClient.LabelValuesQueryAsync(new LableValueQueryRequest
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
        var data = await _prometheusClient.SeriesQueryAsync(new MetaDataQueryRequest
        {
            Match = query.Match,
            End = query.End.ToUnixTimestamp().ToString(),
            Start = query.Start.ToUnixTimestamp().ToString()
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

    private static Dictionary<string, Dictionary<string, List<string>>> ConverToKeyValues(IEnumerable<IDictionary<string, string>> sources)
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
                {
                    continue;
                }
                else
                {
                    if (dic.ContainsKey(key))
                    {
                        if (!dic[key].Contains(item[key]))
                            dic[key].Add(item[key]);
                    }
                    else
                    {
                        dic.Add(key, new List<string>() { item[key] });
                    }
                }
            }
        }

        return result;
    }
}
