﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Google.Type;
using Masa.Tsc.Service.Admin.Application.Metrics.Queries;
using System.Text;

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

    [EventHandler]
    public async Task GetMultiRangeAsync(MultiRangeQuery query)
    {
        var tasks = new Task<QueryResultCommonResponse>[query.Data.MetricNames.Count];
        var index = 0;
        foreach (var name in query.Data.MetricNames)
        {
            var metric = AppendCondition(name, query.Data.ServiceName, query.Data.Instance, query.Data.EndPoint);
            tasks[index] = _prometheusClient.QueryRangeAsync(new QueryRangeRequest
            {
                End = query.Data.End.ToUnixTimestamp().ToString(),
                Start = query.Data.Start.ToUnixTimestamp().ToString(),
                Query = metric,
                Step = query.Data.Step,
            });
            index++;
        }
        var result = await Task.WhenAll(tasks);
        query.Result = result.Select(item => item.Data!).ToList();
    }

    [EventHandler]
    public async Task GetValuesAsync(ValuesQuery query)
    {
        var metric = "";
        if (string.Equals(query.Data.Type, nameof(query.Data.Service), StringComparison.InvariantCultureIgnoreCase))
            metric = $"group by (service_name) (http_client_duration_bucket{{}})";
        else if (string.Equals(query.Data.Type, nameof(query.Data.Instance), StringComparison.InvariantCultureIgnoreCase))
            metric = $"group by (service_instance_id) (http_client_duration_bucket{{}})";
        else
            metric = $"group by (endpoint) (http_client_duration_bucket{{}})";

        metric = AppendCondition(metric, query.Data.Service, query.Data.Instance, query.Data.Endpoint);

        var result = await _prometheusClient.QueryRangeAsync(new QueryRangeRequest
        {
            End = query.Data.End.ToUnixTimestamp().ToString(),
            Start = query.Data.Start.ToUnixTimestamp().ToString(),
            Query = metric,
            Step = query.Data.Step
        });

        if (result.Status == ResultStatuses.Success)
        {
            if (result.Data == null || result.Data.Result == null || !result.Data.Result.Any())
                return;
            query.Result = result.Data.Result.Select(item => ((QueryResultMatrixRangeResponse)item).Metric.Values.FirstOrDefault()?.ToString()).ToList()!;
        }
    }

    private string AppendCondition(string str, string service, string instance, string endpoint)
    {
        StringBuilder text = new StringBuilder();
        if (!string.IsNullOrEmpty(service))
            text.Append($"service_name=\"{service}\",");
        if (!string.IsNullOrEmpty(instance))
            text.Append($"service_instance_id=\"{instance}\",");
        if (!string.IsNullOrEmpty(endpoint))
            text.Append($"endpoint=\"{endpoint}\",");

        if (text.Length == 0)
            return str;

        int position = str.IndexOf('{');
        if (position > 0)
        {
            return str.Insert(position + 1, text.ToString());
        }

        return str;
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
