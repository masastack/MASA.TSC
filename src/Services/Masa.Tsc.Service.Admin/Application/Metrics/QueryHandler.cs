// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Service.Admin.Application.Metrics.Queries;
using System.Text;
using System.Text.RegularExpressions;

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public class QueryHandler
{
    private readonly IMasaPrometheusClient _prometheusClient;
    private readonly IMultilevelCacheClient _multilevelCacheClient;
    private readonly ILogger _logger;
    private readonly object lockObj = new();

    public QueryHandler(IMasaPrometheusClient masaPrometheusClient, IMultilevelCacheClientFactory multilevelCacheClientFactory, ILogger<QueryHandler> logger)
    {
        _prometheusClient = masaPrometheusClient;
        _multilevelCacheClient = multilevelCacheClientFactory.Create(MasaStackConsts.TSC_SYSTEM_SERVICE_APP_ID);
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
            var metric = await AppendCondition(name, query.Data.ServiceName, query.Data.Instance, query.Data.EndPoint);
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
    public async Task GetMultiQueryAsync(MultiQuery query)
    {
        var tasks = new Task<QueryResultCommonResponse>[query.Data.Queries.Count];
        var index = 0;
        foreach (var name in query.Data.Queries)
        {
            var metric = await AppendCondition(name, query.Data.ServiceName, query.Data.Instance, query.Data.EndPoint);
            tasks[index] = _prometheusClient.QueryAsync(new QueryRequest
            {
                Time = query.Data.Time.ToUnixTimestamp().ToString(),
                Query = metric
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
        if (query.Type == MetricValueTypes.Service)
            metric = $"group by (service_name) (http_client_duration_bucket)";
        else if (query.Type == MetricValueTypes.Instance)
            metric = $"group by (service_instance_id) (http_client_duration_bucket)";
        else if (query.Type == MetricValueTypes.Endpoint)
            metric = $"group by (http_target) (http_response_bucket)";

        metric = await AppendCondition(metric, query.Service, default!, default!);

        var result = await _prometheusClient.QueryAsync(new QueryRequest
        {
            Query = metric,
        });

        if (result.Status == ResultStatuses.Success)
        {
            if (result.Data == null || result.Data.Result == null || !result.Data.Result.Any())
                return;
            query.Result = result.Data.Result.Select(item => ((QueryResultInstantVectorResponse)item).Metric.Values.FirstOrDefault()?.ToString()).ToList()!;
        }
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

    private async Task<List<string>> GetAllMetricsAsync()
    {
        List<string>? data = null;
        lock (lockObj)
        {
            int max = 3;
            do
            {
                try
                {
                    data = _multilevelCacheClient.Get<List<string>>(MetricConstants.ALL_METRICS_KEY);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("GetAllMetricsAsync", ex);
                    max--;
                    Task.WaitAll(Task.Delay(20));
                }
            } while (max > 0);
        }
        if (data == null)
        {
            var result = await _prometheusClient.LabelValuesQueryAsync(new() { Lable = "__name__" });
            if (result.Status == ResultStatuses.Success)
            {
                data = result.Data?.ToList() ?? new();
            }
        }
        if (data != null)
            await _multilevelCacheClient.SetAsync(MetricConstants.ALL_METRICS_KEY, data, new CacheEntryOptions(new DateTimeOffset(DateTime.UtcNow.AddMinutes(5))));
        return data!;
    }

    private async Task<string> AppendCondition(string str, string service, string instance, string endpoint)
    {
        var metrics = await GetAllMetricsAsync();
        if (metrics == null || !metrics.Any())
            return str;

        StringBuilder text = new();
        if (!string.IsNullOrEmpty(service))
            text.Append($"service_name=\"{service}\",");
        if (!string.IsNullOrEmpty(instance))
            text.Append($"service_instance_id=\"{instance}\",");
        if (!string.IsNullOrEmpty(endpoint))
            text.Append($"endpoint=\"{endpoint}\",");

        if (text.Length == 0)
            return str;

        foreach (var metric in metrics)
        {
            if (ReplaceMetric(str, metric, text.ToString(), out var result))
                str = result;
        }

        return str;
    }

    private bool ReplaceMetric(string str, string metric, string replace, out string result)
    {
        result = default!;
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(replace))
            return false;
        int start = 0, itemLenth = metric.Length;
        List<int> positions = new List<int>();
        var not = new Regex("_[a-zA-Z0-9]");
        do
        {
            var index = str.IndexOf(metric, start);
            if (index < 0)
                return false;

            if ((index == 0 || index > 0 && !not.IsMatch(str[index - 1].ToString())) && (str.Length - index == 0 || str.Length - index > 0 && !not.IsMatch(str[index + 1].ToString())))
                positions.Add(index);
            start = index + itemLenth;
            if (str.Length - start - itemLenth < 0)
                break;
        } while (true);

        if (positions.Count == 0)
            return false;

        start = positions.Count - 1;
        StringBuilder text = new StringBuilder(str);
        do
        {
            var position = positions[start] + itemLenth;
            bool has = str.Length - position - 1 < 0 ? false : str[position] == '{';
            if (!has)
                text.Insert(position, '{');

            text.Insert(position + 1, replace);

            if (!has)
                text.Insert(position + 1 + replace.Length, '}');

            start--;
        }
        while (start >= 0);

        result = text.ToString();
        return true;
    }
}