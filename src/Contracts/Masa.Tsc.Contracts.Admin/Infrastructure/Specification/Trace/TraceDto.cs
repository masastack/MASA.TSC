// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TraceDto
{
    [JsonPropertyName("@timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("EndTimestamp")]
    public DateTime EndTimestamp { get; set; }

    public string TraceId { get; set; }

    public string SpanId { get; set; }

    public string ParentSpanId { get; set; }

    public string Kind { get; set; }

    public int TraceStatus { get; set; }

    public string Name { get; set; }

    public Dictionary<string, object> Attributes { get; set; }

    public Dictionary<string, object> Resource { get; set; }

    public long GetDuration()
    {
        return (long)Math.Floor((EndTimestamp - Timestamp).TotalMilliseconds);
    }

    public static string GetDispalyName(TraceDto dto)
    {
        if (dto.IsHttp(out var traceHttpDto))
        {
            if (dto.Kind == TraceDtoKind.SPAN_KIND_SERVER)
                return traceHttpDto.Target;
            return traceHttpDto.Url;
        }
        else if (dto.IsDatabase(out var databaseDto))
        {
            return databaseDto.Name;
        }
        else if (dto.IsException(out TraceExceptionDto exceptionDto))
        {
            return exceptionDto.Type ?? exceptionDto.Message;
        }
        else
            return dto.Name;
    }
}

public sealed class TraceDtoKind
{
    private TraceDtoKind() { }

    public const string SPAN_KIND_SERVER = nameof(SPAN_KIND_SERVER);

    public const string SPAN_KIND_CLIENT = nameof(SPAN_KIND_CLIENT);
}

public sealed class TraceDtoType
{
    private TraceDtoType() { }

    public const string Http = nameof(Http);
    public const string Database = nameof(Database);
}