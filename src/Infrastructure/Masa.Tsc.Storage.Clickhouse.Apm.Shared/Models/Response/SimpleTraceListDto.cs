// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models.Response;

public class SimpleTraceListDto
{
    public string TraceId { get; set; }

    public DateTime Timestamp { get; set; }

    public DateTime EndTimestamp { get; set; }
}
