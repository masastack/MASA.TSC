// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json.Serialization;

namespace Masa.Tsc.Observability.Elastic;

public class TraceBaseDto
{
    [JsonPropertyName("@timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("trace.id")]
    public string TraceId { get; set; }

    [JsonPropertyName("transaction.id")]
    public string SpanId { get; set; }

    [JsonPropertyName("service.name")]
    public string ServiceName { get; set; }

    [JsonPropertyName("transaction.duration.us")]
    public int Duration { get; set; }

    [JsonPropertyName("transaction.name")]
    public string Endpoint { get; set; }
}
