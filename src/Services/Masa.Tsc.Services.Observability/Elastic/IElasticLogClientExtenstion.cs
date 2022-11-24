//// Copyright (c) MASA Stack All rights reserved.
//// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

//namespace Nest;

//public static class IElasticLogClientExtenstion
//{
//    public static async Task SearchLogAsync<TQuery>(this IElasticClient client,       
//        TQuery query,
//        Func<QueryContainerDescriptor<LogDto>, TQuery, QueryContainer>? condition = null,
//        Action<ISearchResponse<LogDto>, TQuery>? result = null,
//        Func<AggregationContainerDescriptor<LogDto>, TQuery, IAggregationContainer>? aggregate = null,
//        Func<ValueTuple<bool, int, int>>? page = null,
//        Func<SortDescriptor<LogDto>, TQuery, SortDescriptor<LogDto>>? sort = null,
//        Func<string[]>? includeFields = null,
//        Func<string[]>? excludeFields = null,
//        ILogger? logger = null
//        ) where TQuery : class
//    {
//        await client.SearchAsync(ElasticConst.LogIndex,query, condition, result, aggregate, page, sort, includeFields, excludeFields, logger);
//    }

  

//}