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
}