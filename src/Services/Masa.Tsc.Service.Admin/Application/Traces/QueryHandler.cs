// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Application.Traces;

public class QueryHandler
{
    private readonly IElasticClient _elasticClient;
    private readonly IServiceProvider _provider;
    private readonly ILogger _logger;

    public QueryHandler(IServiceProvider provider, IElasticClient elasticClient, ILogger<QueryHandler> logger)
    {
        _provider = provider;
        _elasticClient = elasticClient;
        _logger = logger;
    }

    [EventHandler]
    public async Task GetDetailAsync(TraceDetailQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceDetailQuery>($"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}", query,
            condition: (container, q) => container.Term(t => t.Field(ElasticConst.TRACE_ID).Value(q.TraceId)),
            sort: (sort, q) => sort.Ascending(ElasticConst.TraceTimestamp),
            page: () => ValueTuple.Create(false, 0, ElasticConst.MAX_DATA_COUNT - 1),
            result: (rep, q) => q.Result = rep.Documents!,
            logger: _logger);
    }

    [EventHandler]
    public async Task GetListAsync(TraceListQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceListQuery>($"{ElasticConst.TraceIndex}", query,
            condition: (container, q) =>
            {
                var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
                if (!string.IsNullOrEmpty(query.TraceId))
                {
                    list.Add(t => t.Term(f => f.Value(q.TraceId).Field(ElasticConst.TRACE_ID)));
                }
                if (!string.IsNullOrEmpty(q.Service))
                {
                    list.Add(t => t.Term(f => f.Value(q.Service).Field(ElasticConst.TRACE_SERVICE_NAME)));
                }
                if (!string.IsNullOrEmpty(q.Instance))
                {
                    list.Add(t => t.Term(f => f.Value(q.Instance).Field(ElasticConst.TRACE_INSTANCE_NAME)));
                }
                if (!string.IsNullOrEmpty(q.Service))
                {
                    list.Add(t => t.Term(f => f.Value(q.Endpoint).Field(ElasticConst.TRACE_ENDPOINT_NAME)));
                }

                list.Add(t => t.DateRange(f => f.LessThan(q.End).GreaterThan(q.Start).Field(ElasticConst.TraceTimestamp)));
                return container.Bool(t => t.Must(list));
            },
            sort: (sort, q) => sort.Ascending(ElasticConst.TraceTimestamp),
            page: () => ValueTuple.Create(true, query.Page, query.Size),
            result: (rep, q) => q.Result = new PaginationDto<object>(rep.Total, rep.Documents?.ToList()!),
            logger: _logger);
    }

    [EventHandler]
    public async Task GetAttrValuesAsync(TraceAttrValuesQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceAttrValuesQuery>($"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}", query,
            condition: (container, q) =>
            {
                var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
                if (query.Queries != null)
                {
                    foreach (var item in q.Queries)
                    {
                        list.Add(t => t.Term(f => f.Value(item.Value).Field(item.Key)));
                    }
                }
                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    list.Add(t => t.Wildcard(f => f.Value($"{q.Keyword}*").Field(q.Name)));
                }

                list.Add(t => t.DateRange(f => f.LessThan(q.End).GreaterThan(q.Start).Field(ElasticConst.TraceTimestamp)));
                return container.Bool(t => t.Must(list));
            },
            aggregate: (agg, q) => agg.Terms(q.Name, f => f.Field(q.Name).Size(q.Limit)),
            sort: (sort, q) => sort.Ascending(q.Name),
            result: (rep, q) =>
            {
                var bucket = (BucketAggregate)rep.Aggregations[q.Name];
                var data = bucket.Items.Select(item => ((KeyedBucket<object>)item).Key.ToString()).ToList();
                data.Sort();
                q.Result = data!;
            },
            page: () => ValueTuple.Create(false, 0, query.Limit),
            logger: _logger);
    }

    [EventHandler]
    public async Task GetAggregationAsync(TraceAggregationQuery query)
    {
        string index = string.Empty;
        var find = query.Queries.FirstOrDefault(item => string.Equals(item.Key, "isSpan", StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrEmpty(find.Key))
        {
            if (bool.Parse(find.Value))
                index += $",{ElasticConst.SpanIndex}";
            query.Queries.Remove(find.Key);
        }
        find = query.Queries.FirstOrDefault(item => string.Equals(item.Key, "isTrace", StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrEmpty(find.Key))
        {
            if (bool.Parse(find.Value))
                index += $",{ElasticConst.SpanIndex}";
            query.Queries.Remove(find.Key);
        }
        if (index.Length > 0)
        {
            index = index[1..];
        }
        else
        {
            index = $"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}";
        }

        await _elasticClient.SearchAsync<object, TraceAggregationQuery>(index, query,
            condition: (container, q) =>
            {
                var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
                foreach (var item in q.Queries)
                {
                    list.Add(t => t.Term(f => f.Value(item.Value).Field(item.Key)));
                }

                list.Add(t => t.DateRange(f => f.LessThan(q.End).GreaterThan(q.Start).Field(ElasticConst.TraceTimestamp)));
                return container.Bool(t => t.Must(list));
            },
            sort: (sort, q) => sort.Descending(ElasticConst.TraceTimestamp),
            result: (rep, q) =>
            {
                var data = IElasticClientExtenstion.AggResult(rep, q.Fields);
                q.Result = new ChartLineDataDto<ChartPointDto>();
                if (data != null && data.Any())
                    q.Result.Data = data.Select(item => new ChartPointDto
                    {
                        X = item.Key,
                        Y = item.Value,
                    });
            },
            aggregate: (agg, q) => IElasticClientExtenstion.Aggregation(agg, q.Fields, q.Interval),
            logger: _logger);
    }

}