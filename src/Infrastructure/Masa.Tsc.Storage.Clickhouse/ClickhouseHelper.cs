// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse;

public sealed class ClickhouseHelper
{
    private ClickhouseHelper() { }

    public const string ATTRIBUTE_KEY = "Attributes.";
    public const string RESOURCE_KEY = "Resource.";

    public static string GetName(string name, bool isLog)
    {
        if (name.Equals("@timestamp", StringComparison.CurrentCultureIgnoreCase) || name.Equals(StorageConstaaa.Current.Timestimap, StringComparison.CurrentCultureIgnoreCase))
            return StorageConstaaa.Current.Timestimap;

        if (!isLog && name.Equals(StorageConstaaa.Current.Trace.Duration, StringComparison.CurrentCultureIgnoreCase))
            return StorageConstaaa.Current.Trace.Duration;

        if (!isLog && (name.Equals("kind", StringComparison.InvariantCultureIgnoreCase) || name.Equals(StorageConstaaa.Current.Trace.SpanKind, StringComparison.InvariantCultureIgnoreCase)))
            return StorageConstaaa.Current.Trace.SpanKind;

        if (name.StartsWith(RESOURCE_KEY, StringComparison.CurrentCultureIgnoreCase))
            return GetResourceName(name);

        if (name.StartsWith(ATTRIBUTE_KEY, StringComparison.CurrentCultureIgnoreCase))
            return GetAttributeName(name, isLog);

        return name;
    }

    public static string GetResourceName(string name)
    {
        var field = name[(RESOURCE_KEY.Length)..];
        if (field.Equals("service.name", StringComparison.CurrentCultureIgnoreCase))
            return "ServiceName";

        if (field.Equals("service.namespace", StringComparison.CurrentCultureIgnoreCase) || field.Equals("service.instance.id", StringComparison.CurrentCultureIgnoreCase))
            return $"{RESOURCE_KEY}{field.ToLower()}";

        return $"ResourceAttributesValues[indexOf(ResourceAttributesKeys,'{field}')]";
    }

    public static string GetAttributeName(string name, bool isLog)
    {
        var pre = isLog ? "Log" : "Span";
        var field = name[(ATTRIBUTE_KEY.Length)..];
        if (isLog && (field.Equals("exception.message", StringComparison.CurrentCultureIgnoreCase)))
            return $"{ATTRIBUTE_KEY}{field}";

        if (!isLog && (
               field.Equals("http.target", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.method", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.status_code", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.url", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("enduser.id", StringComparison.CurrentCultureIgnoreCase))
            || field.Equals("http.request_content_body", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.response_content_body", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("exception.message", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("exception.type", StringComparison.CurrentCultureIgnoreCase)
            )
            return $"{ATTRIBUTE_KEY}{field}";

        return $"{pre}AttributesValues[indexOf({pre}AttributesKeys,'{field}')]";
    }

    public static string AppendLike(string field, string name, string text)
    {
        if (Regex.IsMatch(text, @"^[\da-zA-Z]+$"))
        {
            return $" and hasToken({field},'{text}')";
        }
        return $" and {field} like @{name}";
    }

    public static ClickHouseParameter GetLikeParameter(string name, string text)
    {
        if (Regex.IsMatch(text, ""))
        {
            text = $"%{text}%";
        }
        return new ClickHouseParameter() { ParameterName = name, Value = $"{text}" };
    }

}
