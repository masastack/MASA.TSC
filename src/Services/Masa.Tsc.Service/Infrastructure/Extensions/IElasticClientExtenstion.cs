// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest
{
    public static class IElasticClientExtenstion
    {
        public static async Task<T> GetAggregation<T, Q>(this IElasticClient client, string index, Q q,
            Func<QueryContainerDescriptor<T>, Q, QueryContainer> queryFn,
            Func<AggregationContainerDescriptor<T>, Q, IAggregationContainer> aggFn,
            Func<AggregateDictionary, T> resultFn
            ) where T : class where Q : class

        {
            var rep = await client.SearchAsync<T, Q>(s => s.Index(index).Query(query => queryFn?.Invoke(query, q)).Aggregations(agg => aggFn?.Invoke(agg, q)));
            if (rep.IsValid)
            {
                return resultFn.Invoke(rep.Aggregations);
            }
            else
            {
                return default(T);
            }
        }
    }
}
