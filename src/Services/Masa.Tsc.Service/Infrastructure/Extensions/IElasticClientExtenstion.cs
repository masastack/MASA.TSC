// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Caller.Core;
using System.Text.Json.Nodes;

namespace Nest;

public static class IElasticClientExtenstion
{
    public static async Task GetAggregationAsync<T, Q>(this IElasticClient client,
        string indexName,
        Q q,
        Func<QueryContainerDescriptor<T>, Q, QueryContainer> queryFn,
        Func<AggregationContainerDescriptor<T>, Q, IAggregationContainer> aggFn,
        Action<ISearchResponse<T>,Q> resultFn
        ) where T : class where Q : class
    {
        var rep = await client.SearchAsync<T>(s => s.Index(indexName).Query(query => queryFn?.Invoke(query, q)).Size(1).Aggregations(agg => aggFn?.Invoke(agg, q)));
        resultFn?.Invoke(rep,q);
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
        var result = await caller.GetAsync<JsonObject>(path, false, token);
        if (result == null)
            return default;

        var root = result.FirstOrDefault(attr => string.Equals(attr.Key, indexName, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(root.Key))
            return default;

        return GetProperties(root.Value, null);
    }

    private static IEnumerable<MappingResponse>? GetProperties(JsonNode? node, string? parentName = default)
    {
        if (node == null)
            return default;
        var properties = GetProperties(node);
        if (properties == null)
            return default;

        var obj = properties.AsObject();
        var result = new List<MappingResponse>();
        foreach (var item in obj)
        {
            var type = GetType(item.Value);
            var childProperties = GetProperties(item.Value);
            var name = $"{parentName}{item.Key}";
            if (string.IsNullOrEmpty(type))
            {
                if (childProperties == null)
                    continue;
                var children = GetProperties(item.Value, $"{name}.");
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

    private static JsonNode? GetProperties(JsonNode? value)
    {
        return value?[MappingConst.PROPERTY];
    }

    private static string? GetType(JsonNode? value)
    {
        var find = value?[MappingConst.TYPE];
        if (find == null)
            return default;

        return find.ToString();
    }

    private static void SetKeyword(JsonNode? value, MappingResponse model)
    {
        var fields = value?[MappingConst.FIELD];
        if (fields == null)
            return;

        var find = fields[MappingConst.KEYWORD];
        if (find == null)
            return;
        var obj = find.AsObject();
        if (obj.ContainsKey(MappingConst.TYPE) && find[MappingConst.TYPE]?.ToString() == MappingConst.KEYWORD)
            model.IsKeyword = true;
        if (obj.ContainsKey(MappingConst.MAXLENGTH))
            model.MaxLenth = Convert.ToInt32(find[MappingConst.MAXLENGTH]);
    }
}
