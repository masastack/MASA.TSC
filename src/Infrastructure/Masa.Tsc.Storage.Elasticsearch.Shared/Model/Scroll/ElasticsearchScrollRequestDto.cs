// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Elasticsearch.Model;

public class ElasticsearchScrollRequestDto : BaseRequestDto
{
    public string Scroll { get; set; }

    public string ScrollId { get; set; }
}
