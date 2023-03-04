// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Aggregates;

/// <summary>
/// 调用过程信息
/// </summary>
public class TraceServiceState
{
    [Keyword]
    public string ServiceId { get; set; }

    [Keyword]
    public string ServiceName { get; set; }

    [Keyword]
    public string Instance { get; set; }

    [Keyword]
    public string DestEndpint { get; set; }

    [Keyword]
    public string DestInstance { get; set; }

    [Keyword]
    public string DestServiceId { get; set; }

    [Keyword]
    public string DestServiceName { get; set; }

    public DateTime Timestamp { get; set; }

    public bool IsSuccess { get; set; }

    /// <summary>
    /// unit us
    /// </summary>    
    public long Latency { get; set; }
}