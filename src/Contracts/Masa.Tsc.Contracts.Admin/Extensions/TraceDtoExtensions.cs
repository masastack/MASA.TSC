// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public static class TraceDtoExtensions
{
    private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static bool IsHttp(this TraceDto data, out TraceHttpDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "http.method" }))
            return false;
        result = data.Attributes.Deserialize<TraceHttpDto>();

        result.RequestHeaders = data.Attributes.ConvertDic("http.request.header.", asdasdasd);
        result.ReponseHeaders = data.Attributes.ConvertDic("http.response.header.", asdasdasd);

        result.Name = data.Name;
        result.Status = data.TraceStatus;
        return true;
    }

    private static IEnumerable<string> asdasdasd(object obj)
    {
        var value = (JsonElement)obj;


        if (value.ValueKind == JsonValueKind.Array)
        {
            return value.EnumerateArray().Select(item => item.ToString()).ToArray();
        }
        else
        {
            return new string[] { value.ToString() };
        }
    }

    public static bool IsDatabase(this TraceDto data, out TraceDatabaseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "db.system" }))
            return false;
        result = data.Attributes.Deserialize<TraceDatabaseDto>();
        return true;
    }

    public static bool IsException(this TraceDto data, out TraceExceptionDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "exception.type", "exception.message" }))
            return false;

        result = data.Attributes.Deserialize<TraceExceptionDto>();

        return true;
    }

    private static bool ContainsKeies(this Dictionary<string, object> dic, IEnumerable<string> keys)
    {
        return dic != null && dic.Any() && dic.Keys.Any(key => keys.Any(k => key == k));
    }

    private static T Deserialize<T>(this Dictionary<string, object> dic)
    {
        var text = JsonSerializer.Serialize(dic, _serializerOptions);
        return JsonSerializer.Deserialize<T>(text, _serializerOptions)!;
    }

    public static Dictionary<string, T> ConvertDic<T>(this Dictionary<string, object> dic, string prefix, Func<object, T>? convert = null)
    {
        var result = new Dictionary<string, T>();
        foreach (var key in dic.Keys)
        {
            if (!key.StartsWith(prefix))
                continue;
            var value = dic[key];
            var newKey = key[prefix.Length..];
            if (convert != null)
                value = convert(dic[key]);            
            result.Add(newKey, (T)value!);
        }
        return result;
    }
}