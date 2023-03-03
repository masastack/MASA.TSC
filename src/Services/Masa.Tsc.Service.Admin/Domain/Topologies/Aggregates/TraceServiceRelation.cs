// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Aggregates;

/// <summary>
/// 服务依赖关系
/// </summary>
public class TraceServiceRelation
{
    [ElasticsearchNest.Keyword]
    public string ServiceId { get; set; }

    [ElasticsearchNest.Keyword]
    public string DestServiceId { get; set; }
}
