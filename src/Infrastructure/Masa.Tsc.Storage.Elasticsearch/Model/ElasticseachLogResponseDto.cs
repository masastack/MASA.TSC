// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Elasticsearch.Model;

internal class ElasticseachLogResponseDto : LogResponseDto
{
    [JsonPropertyName("@timestamp")]
    public override DateTime Timestamp { get; set; }    
}
