// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest;

public static class IElasticClientExtenstion
{    
    public static async Task SearchAsync<TResult, TQuery>(this IElasticClient client,
        string indexName,
        TQuery query,
        Func<QueryContainerDescriptor<TResult>, TQuery, QueryContainer>? condition = null,
        Action<ISearchResponse<TResult>, TQuery>? result = null,
        Func<AggregationContainerDescriptor<TResult>, TQuery, IAggregationContainer>? aggregate = null,
        Func<ValueTuple<bool, int, int>>? pageration = null,
        Func<SortDescriptor<TResult>, TQuery, SortDescriptor<TResult>>? sort = null,
        Func<string[]>? includeFields = null,
        Func<string[]>? excludeFields = null,
        ILogger? logger = null
        ) where TResult : class where TQuery : class
    {
        try
        {
            SearchDescriptor<TResult> func(SearchDescriptor<TResult> searchDescriptor)
            {
                if (condition is not null)
                    searchDescriptor = searchDescriptor.Query(queryContainer => condition.Invoke(queryContainer, query));
                int page = 0, pageSize = 0;
                bool hasPage = false;
                if (pageration != null)
                {
                    var pageData = pageration.Invoke();
                    hasPage = pageData.Item1;
                    page = pageData.Item2;
                    pageSize = pageData.Item3;
                }
                searchDescriptor = SetPageSize(searchDescriptor, hasPage, page, pageSize);
                if (sort != null)
                    searchDescriptor = searchDescriptor.Sort(sortDescriptor => sort(sortDescriptor, query));
                if (includeFields != null || excludeFields != null)
                {
                    searchDescriptor = searchDescriptor.Source(source =>
                    {
                        if (includeFields != null)
                            source = source.Includes(f => f.Fields(includeFields.Invoke()));
                        if (excludeFields != null)
                            source = source.Excludes(f => f.Fields(excludeFields.Invoke()));
                        return source;
                    });
                }

                if (aggregate != null)
                {
                    searchDescriptor = searchDescriptor.Aggregations(agg => aggregate?.Invoke(agg, query));
                }
                return searchDescriptor;
            }
            var searchResponse = await client.SearchAsync<TResult>(s => func(s.Index(indexName)));
            searchResponse.FriendlyElasticException("SearchAsync", logger);
            if (searchResponse.IsValid)
            {
                result?.Invoke(searchResponse, query);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("{0} is error {1}", "SearchAsync", ex);
            throw new UserFriendlyException($"SearchAsync execute error {ex.Message}");
        }
    }

    private static SearchDescriptor<T> SetPageSize<T>(SearchDescriptor<T> container, bool hasPage, int page, int size) where T : class
    {
        if (!hasPage)
            return container.Size(size);

        var start = (page - 1) * size;

        if (ElasticConst.MAX_DATA_COUNT - start - size <= 0)
            throw new UserFriendlyException($"elastic query data max count must be less {ElasticConst.MAX_DATA_COUNT}, please input more condition to limit");

        return container.Size(size).From(start);
    }

    /// <summary>
    /// 获取mapping
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="indexName"></param>                    
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<MappingResponse>?> GetMappingAsync(this ICallerProvider caller, string indexName, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(indexName, nameof(indexName));
        var path = $"/{indexName}/_mapping";
        var result = await caller.GetAsync<JsonElement>(path, false, token);

        if (!result.TryGetProperty(indexName, out JsonElement root) ||
            !root.TryGetProperty("mappings", out JsonElement mapping))
        {
            return default;
        }

        return GetRepProperties(mapping, null);
    }

    private static IEnumerable<MappingResponse>? GetRepProperties(JsonElement node, string? parentName = default)
    {
        if (node.ValueKind != JsonValueKind.Object)
            return default;

        var properties = GetProperties(node);
        if (properties == null || properties.Value.ValueKind != JsonValueKind.Object)
            return default;

        var result = new List<MappingResponse>();
        var obj = properties.Value.EnumerateObject();
        foreach (var item in obj)
        {
            var type = GetType(item.Value);
            var name = $"{parentName}{item.Name}";
            if (string.IsNullOrEmpty(type))
            {
                var children = GetRepProperties(item.Value, $"{name}.");
                if (children != null && children.Any())
                    result.AddRange(children);
            }
            else
            {
                var model = new MappingResponse
                {
                    Name = name,
                    DataType = type
                };
                SetKeyword(item.Value, model);
                result.Add(model);
            }
        }
        return result;
    }

    private static JsonElement? GetProperties(JsonElement value)
    {
        if (value.TryGetProperty(MappingConst.PROPERTY, out JsonElement result))
            return result;
        return null;
    }

    private static string? GetType(JsonElement value)
    {
        if (value.TryGetProperty(MappingConst.TYPE, out JsonElement find))
            return find.ToString();
        return default;
    }

    private static void SetKeyword(JsonElement value, MappingResponse model)
    {
        if (value.TryGetProperty(MappingConst.FIELD, out JsonElement fields) &&
       fields.TryGetProperty(MappingConst.KEYWORD, out JsonElement find))
        {
            if (find.TryGetProperty(MappingConst.TYPE, out JsonElement type) && type.ToString() == MappingConst.KEYWORD)
                model.IsKeyword = true;
            if (find.TryGetProperty(MappingConst.MAXLENGTH, out JsonElement maxLength))
                model.MaxLenth = maxLength.GetInt32();
        }
    }

    public static void FriendlyElasticException<T>(this ISearchResponse<T> response, string callerName, ILogger? logger) where T : class
    {
        if (!response.IsValid)
        {
            logger?.LogError("{0} Error {1}", callerName, response);
            throw new UserFriendlyException($"elastic query error: status:{response.ServerError?.Status},message:{response.OriginalException?.Message ?? response.ServerError?.ToString()}");
        }
    }
}
