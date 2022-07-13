// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json;
using System.Text.RegularExpressions;

namespace System;

public static class JsonElementExtensions
{
    public static IEnumerable<KeyValuePair<string, object>>? ToKeyValuePairs(this JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Object)
            return null;

        return GetObject(value);
    }

    private static object? GetValue(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                return GetObject(value);
            case JsonValueKind.Array:
                return GetArray(value);
            case JsonValueKind.String:
                return value.GetString();
            case JsonValueKind.Number:
                return GetNumber(value);
            case JsonValueKind.True:
            case JsonValueKind.False:
                return value.GetBoolean();
            default:
                return null;
        }
    }

    private static object GetNumber(JsonElement value)
    {
        var str = value.GetRawText();

        if (Regex.IsMatch(str, @"\."))
        {
            return value.GetDouble();
        }
        else
        {
            var num = value.GetInt32();
            var num64 = value.GetInt64();
            if (num - num64 != 0)
                return num64;
            return num;
        }
    }

    private static IEnumerable<KeyValuePair<string, object>>? GetObject(JsonElement value)
    {
        var result = new List<KeyValuePair<string, object>>();
        foreach (var item in value.EnumerateObject())
        {
            var v = GetValue(item.Value);
            if (v == null)
                continue;
            result.Add(KeyValuePair.Create(item.Name, v));
        }
        if (result.Any())
            return result;
        return null;
    }

    private static IEnumerable<object?> GetArray(JsonElement value)
    {
        var temp = value.EnumerateArray();
        if (temp.Any())
            return default!;
        var list = new List<object?>();
        foreach (var item in temp)
        {
            var v = GetValue(item);
            list.Add(v);
        }
        return list;
    }

    public static DateTime? GetTimestamp(this JsonElement value, string timeSpanKey= "@timestamp")
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            var tmp = value.EnumerateObject();
            var find = tmp.FirstOrDefault(m => string.Equals(m.Name, timeSpanKey, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(find.Name))
            {
                if (find.Value.ValueKind == JsonValueKind.Number)
                    return find.Value.GetInt64().ToDateTime();
                else if (find.Value.ValueKind == JsonValueKind.String)
                    if (DateTime.TryParse(find.Value.GetString(), out DateTime time))
                        return time;
            }
        }

        return default;
    }
}