// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Elasticsearch.Model;

internal class ElasticseachTraceResponseDto : TraceResponseDto
{
    [JsonPropertyName("@timestamp")]
    public override DateTime Timestamp { get; set; }

    [JsonPropertyName("EndTimestamp")]
    public override DateTime EndTimestamp { get; set; }    
}
