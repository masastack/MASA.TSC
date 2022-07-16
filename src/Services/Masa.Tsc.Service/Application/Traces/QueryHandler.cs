// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Application.Traces;

public class QueryHandler
{
    private readonly IElasticClient _elasticClient;
    private readonly IServiceProvider _provider;
    private readonly ILogger _logger;
    private readonly ICallerProvider _caller;

    public QueryHandler(IServiceProvider provider, IElasticClient elasticClient, ILogger<QueryHandler> logger)
    {
        _provider = provider;
        _elasticClient = elasticClient;
        _logger = logger;
        _caller = _provider.GetRequiredService<ICallerFactory>().CreateClient(ElasticConst.ES_HTTP_CLIENT_NAME);
    }

    [EventHandler]
    public async Task GetDetailAsync(TraceDetailQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceDetailQuery>($"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}", query,
            queryFn: (container, q) =>
            {
                return container.Term(t => t.Field("traceId").Value(query.TraceId));
            },
            sortFn: (sort, q) =>
            {
                return sort.Ascending(ElasticConst.TraceTimestamp);
            },
            resultFn: (rep, q) =>
            {
                q.Result = rep.Documents!;
            },
            logger: _logger);
    }

    [EventHandler]
    public async Task GetListAsync(TraceListQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceListQuery>($"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}", query,
            queryFn: (container, q) =>
            {
                var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
                if (!string.IsNullOrEmpty(query.TraceId))
                {
                    list.Add(t => t.Term(f => f.Value(q.TraceId).Field(ElasticConst.TraceId)));
                }
                if (!string.IsNullOrEmpty(q.Service))
                {
                    list.Add(t => t.Term(f => f.Value(q.Service).Field(ElasticConst.TraceServiceName)));
                }
                if (!string.IsNullOrEmpty(q.Instance))
                {
                    list.Add(t => t.Term(f => f.Value(q.Instance).Field(ElasticConst.TraceInstanceName)));
                }
                if (!string.IsNullOrEmpty(q.Service))
                {
                    list.Add(t => t.Term(f => f.Value(q.Endpoint).Field(ElasticConst.TraceEndpointName)));
                }

                list.Add(t => t.DateRange(f => f.LessThan(q.Start).GreaterThan(q.End).Field(ElasticConst.TraceTimestamp)));
                return container.Bool(t => t.Must(list));
            },
            sortFn: (sort, q) =>
            {
                return sort.Ascending(ElasticConst.TraceTimestamp);
            },
            pageFn: () => Tuple.Create(true, query.Page, query.Size),
            resultFn: (rep, q) =>
            {
                q.Result = new PaginationDto<object>(rep.Total, rep.Documents?.ToList()!);
            },
            logger: _logger);
    }

    [EventHandler]
    public async Task GetAttrValuesAsync(TraceAttrValuesQuery query)
    {
        await _elasticClient.SearchAsync<object, TraceAttrValuesQuery>($"{ElasticConst.TraceIndex},{ElasticConst.SpanIndex}", query,
            queryFn: (container, q) =>
            {
                var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
                foreach (var item in q.Query)
                {
                    list.Add(t => t.Term(f => f.Value(item.Value).Field(item.Key)));
                }

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    list.Add(t => t.Wildcard(f => f.Value(q.Keyword).Field(q.Name)));
                }


                list.Add(t => t.DateRange(f => f.LessThan(q.Start).GreaterThan(q.End).Field(ElasticConst.TraceTimestamp)));
                return container.Bool(t => t.Must(list));
            },
            sortFn: (sort, q) =>
            {
                return sort.Ascending(query.Name);
            },
            includeFieldsFn: () => new string[] { query.Name },
            resultFn: (rep, q) =>
            {
                q.Result = rep.Documents?.Select(m =>
                {
                    var obj = ((JsonElement)m).EnumerateObject();
                    foreach (var item in obj)
                    {
                        if (string.Equals(item.Name, query.Name, StringComparison.CurrentCultureIgnoreCase))
                            return item.Value.GetRawText();
                        break;
                    }
                    return default!;
                })!;
            },
            pageFn: () => Tuple.Create(false, 0, query.Limit),
            logger: _logger);
    }

}