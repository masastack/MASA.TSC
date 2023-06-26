// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public class QueryHandler
{
    private readonly IMasaPrometheusClient _prometheusClient;
    private readonly IMultilevelCacheClient _multilevelCacheClient;
    private readonly ILogger _logger;

    public QueryHandler(IMasaStackConfig masaStackConfig, IMasaPrometheusClient masaPrometheusClient, IMultilevelCacheClientFactory multilevelCacheClientFactory, ILogger<QueryHandler> logger)
    {
        _prometheusClient = masaPrometheusClient;
        _multilevelCacheClient = multilevelCacheClientFactory.Create(masaStackConfig.GetServiceId(MasaStackConstant.TSC));
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
            if (data.Data == null || !data.Data.Any())
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
            if (data.Data == null || !data.Data.Any())
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
        var result = new List<QueryResultCommonResponse>();
        foreach (var name in query.Data.MetricNames)
        {
            var metric = await ReplaceCondition(name, query.Data.Layer!, query.Data.Service!, query.Data.Instance!, query.Data.EndPoint!);
            tasks[index] = _prometheusClient.QueryRangeAsync(new QueryRangeRequest
            {
                End = query.Data.End.ToUnixTimestamp().ToString(),
                Start = query.Data.Start.ToUnixTimestamp().ToString(),
                Query = metric,
                Step = query.Data.Step,
            });
            result.Add(await tasks[index]);
            index++;
        }
        query.Result = result.Select(item => item.Data!).ToList();
    }

    [EventHandler]
    public async Task GetMultiQueryAsync(MultiQuery query)
    {
        var tasks = new Task<QueryResultCommonResponse>[query.Data.Queries.Count];
        var index = 0;
        var result = new List<QueryResultCommonResponse>();
        foreach (var name in query.Data.Queries)
        {
            var metric = await ReplaceCondition(name, query.Data.Layer!, query.Data.Service!, query.Data.Instance!, query.Data.EndPoint!);
            tasks[index] = _prometheusClient.QueryAsync(new QueryRequest
            {
                Time = query.Data.Time.ToUnixTimestamp().ToString(),
                Query = metric
            });
            result.Add(await tasks[index]);
            index++;
        }
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
        else if (query.Type == MetricValueTypes.Layer)
            metric = $"group by (service_layer) (http_client_duration_bucket)";

        metric = await ReplaceCondition(metric, query.Layer, query.Service, query.Instance, query.Endpint);

        var result = await _prometheusClient.QueryAsync(new QueryRequest
        {
            Query = metric,
        });

        if (result.Status == ResultStatuses.Success)
        {
            if (!(result.Data == null || result.Data.Result == null || !result.Data.Result.Any()))
            {
                query.Result = result.Data.Result.Select(item => ((QueryResultInstantVectorResponse)item)!.Metric!.Values.FirstOrDefault()?.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToList()!;
                query.Result.Sort();
            }
        }

        if (query.Type == MetricValueTypes.Layer)
        {
            if (query.Result == null || !query.Result.Any())
                query.Result = new List<string> { MetricConstants.DEFAULT_LAYER };
            else
            {
                if ((!query.Result.Contains(MetricConstants.DEFAULT_LAYER)))
                    query.Result.Add(MetricConstants.DEFAULT_LAYER);
                if ((!query.Result.Contains(MetricConstants.DAPR_LAYER)))
                    query.Result.Add(MetricConstants.DAPR_LAYER);
                query.Result.Sort();
            }
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

    private async Task<string> ReplaceCondition(string expression, string layer, string service, string instance, string endpoint)
    {
        var template = _multilevelCacheClient.GetMetricTemplateAsync(expression, _logger);
        if (string.IsNullOrEmpty(template))
        {
            var allMetrics = await _multilevelCacheClient.GetAllMetricsAsync(_prometheusClient, _logger);
            template = InsertMetricsTemplate(expression, allMetrics, MetricConstants.APPEND_TEMPLATE);
        }
        if (template.Contains(MetricConstants.APPEND_TEMPLATE))
        {
            return template.Replace(MetricConstants.APPEND_TEMPLATE, CombineCondition(layer, service, instance, endpoint));
        }
        return template;
    }

    private static string CombineCondition(string layer, string service, string instance, string endpoint)
    {
        StringBuilder text = new();
        if (!string.IsNullOrEmpty(service))
            text.Append($"service_name=\"{service}\",");
        if (!string.IsNullOrEmpty(instance))
            text.Append($"service_instance_id=\"{instance}\",");
        if (!string.IsNullOrEmpty(endpoint))
            text.Append($"endpoint=\"{endpoint}\",");
        if (!string.IsNullOrEmpty(layer) && !string.Equals(MetricConstants.DAPR_LAYER, layer, StringComparison.InvariantCultureIgnoreCase))
            text.Append($"service_layer=\"{layer}\",");
        return text.ToString();
    }

    private string InsertMetricsTemplate(string expresstion, IEnumerable<string> metrics, string template)
    {
        if (string.IsNullOrEmpty(template) || metrics == null || !metrics.Any())
            return expresstion;
        metrics = metrics.Where(s => expresstion.Contains(s, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!metrics.Any())
            return expresstion;

        foreach (var metric in metrics)
        {
            if (InsertTemplate(expresstion, metric, template, out var result))
                expresstion = result;
        }

        return expresstion;
    }

    private bool InsertTemplate(string str, string metric, string replace, out string result)
    {
        result = default!;
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(replace))
            return false;
        int start = 0, itemLenth = metric.Length;
        List<int> positions = new();
        var not = new Regex("[_a-zA-Z0-9]");
        do
        {
            var index = str.IndexOf(metric, start);
            if (index < 0)
                break;

            if (index == 0 && (str.Length - itemLenth == 0 || str.Length - metric.Length > 0 && !not.IsMatch(str[index + itemLenth + 1].ToString())))
            {
                positions.Add(index);
            }
            else if (index > 0 && !not.IsMatch(str[index - 1].ToString()) && (str.Length - index - itemLenth == 0 || str.Length - index > 0 && !not.IsMatch(str[index + itemLenth].ToString())))
            {
                positions.Add(index);
            }

            start = index + itemLenth;
            if (str.Length - start - itemLenth < 0)
                break;
        } while (true);

        if (positions.Count == 0)
            return false;

        start = positions.Count - 1;
        StringBuilder text = new(str);
        do
        {
            var position = positions[start] + itemLenth;
            bool has = str.Length - position - 1 >= 0 && str[position] == '{';
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