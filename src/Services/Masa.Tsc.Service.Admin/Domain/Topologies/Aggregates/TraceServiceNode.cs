// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Aggregates;

/// <summary>
/// 服务节点
/// </summary>
public class TraceServiceNode
{
    [Keyword]
    public string Id { get; set; }

    [Keyword]
    public string Service { get; set; }

    [Keyword]
    public string Instance { get; set; }

    public TraceNodeTypes Type { get; set; }

    public DateTime CreateTime { get; set; }
}