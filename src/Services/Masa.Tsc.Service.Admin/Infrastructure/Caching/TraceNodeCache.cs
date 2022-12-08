// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Caching;

internal class TraceNodeCache
{
    public string ServiceId { get; set; }

    public string CallServiceId { get; set; }

    public string TraceId { get; set; }

    public string SpanId { get; set; }

    public string ParentId { get; set; }

    public string EndPoint { get; set; }

    public string Service { get; set; }

    public string Instance { get; set; }

    public TraceNodeTypes Type { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public bool IsServer { get; set; }

    public bool IsSuccess { get; set; }

    public long Duration =>(long)Math.Floor((End - Start).TotalMilliseconds);
}
