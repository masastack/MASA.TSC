// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Caller.Core;
using Nest;

namespace Masa.Tsc.Service.Admin.Application
{
    public class QueryHandler
    {
        private IElasticClient _elasticClient;
        private ICallerProvider _caller;
        private ILogger _logger;

        public QueryHandler(ICallerProvider caller, IElasticClient elasticClient, ILogger<QueryHandler> logger)
        {
            _caller = caller;
            _elasticClient = elasticClient;
            _logger = logger;
        }

        #region agg query
        [EventHandler]
        public async Task GetAggregationAsync(LogAggQuery query)
        {
            await _elasticClient.GetAggregationAsync<object, LogAggQuery>("index", query, Filter, Aggregation, AggResult);
        }

        private QueryContainer Filter(QueryContainerDescriptor<object> container, LogAggQuery query)
        {
            var list = new List<Func<QueryContainerDescriptor<object>, QueryContainer>>();
            if (!string.IsNullOrEmpty(query.Query))
            {
                list.Add(q => q.Raw(query.Query));
            }
            if (query.Start > DateTime.MinValue && query.End > DateTime.MinValue && query.Start < query.End)
            {
                list.Add(q => q.DateRange(f => f.GreaterThanOrEquals(query.Start).LessThanOrEquals(query.End).Field("@timestamp")));
            }

            if (list.Any())
                container.Bool(b => b.Must(list));

            return container;
        }

        private IAggregationContainer Aggregation(AggregationContainerDescriptor<object> aggContainer, LogAggQuery query)
        {
            if (query.FieldMaps == null || !query.FieldMaps.Any())
                return aggContainer;
            foreach (var item in query.FieldMaps)
            {
                switch (item.AggType)
                {
                    case LogAggTypes.Count:
                        {
                            aggContainer.ValueCount(item.Alias, agg => agg.Field(item.Name));
                        }
                        break;
                    case LogAggTypes.Sum:
                        {
                            aggContainer.Sum(item.Alias, agg => agg.Field(item.Name));
                        }
                        break;
                    case LogAggTypes.Avg:
                        {
                            aggContainer.Average(item.Alias, agg => agg.Field(item.Name));
                        }
                        break;
                }
            }
            return aggContainer;
        }

        private void AggResult(ISearchResponse<object> response, LogAggQuery query)
        {
            if (!response.IsValid)
            {
                if (response.TryGetServerErrorReason(out string msg))
                    throw new UserFriendlyException(msg);
                else
                    _logger.LogError($"Aggregation query error: {0}", response);
            }

            if (response.Aggregations == null || !response.Aggregations.Any())
                return;

            foreach (var item in response.Aggregations)
            {
                //item.Value
            }
        }
        
        #endregion

        [EventHandler]
        public async Task GetLatestDataAsync(LatestLogQuery query)
        {
            var response=await _elasticClient.SearchAsync<object>(q => q.Index("").Query(q => Filter(q, new LogAggQuery { Start = query.Start, End = query.End, Query = query.Query })).Sort(s => {
                if (query.IsDesc)
                    return s.Descending("");
                else
                    return s.Ascending("");
            }).Size(1));
            if (response.IsValid)
            {
                if(response.Documents.Any())
                    query.Result = response.Documents.First();
            }
            else
            {
                _logger.LogError("GetLatestDataAsync Error {0}",response);
            }
        }
    }
}
