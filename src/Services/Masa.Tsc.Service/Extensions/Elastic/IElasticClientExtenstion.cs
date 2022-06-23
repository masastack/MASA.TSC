// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest;

public static class IElasticClientExtenstion
{
    public static async Task GetAggregationAsync<T, Q>(this IElasticClient client,
        string indexName,
        Q q,
        Func<QueryContainerDescriptor<T>, Q, QueryContainer> queryFn,
        Func<AggregationContainerDescriptor<T>, Q, IAggregationContainer> aggFn,
        Action<ISearchResponse<T>, Q> resultFn
        ) where T : class where Q : class
    {
        var rep = await client.SearchAsync<T>(s => s.Index(indexName).Query(query => queryFn?.Invoke(query, q)).Size(1).Aggregations(agg => aggFn?.Invoke(agg, q)));
        resultFn?.Invoke(rep, q);
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
}
