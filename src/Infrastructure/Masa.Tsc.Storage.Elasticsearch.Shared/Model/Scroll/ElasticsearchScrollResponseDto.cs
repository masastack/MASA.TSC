// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Elasticsearch.Model;

public class ElasticsearchScrollResponseDto<TResult>: PaginatedListBase<TResult> where TResult : class
{
    public string ScrollId { get; set; }
}
