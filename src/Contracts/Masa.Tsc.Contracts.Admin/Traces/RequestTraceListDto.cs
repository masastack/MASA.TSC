﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestTraceListDto : Pagination<RequestTraceListDto>
{
    public string TraceId { get; set; }

    public string Service { get; set; }

    public string Instance { get; set; }

    public string Endpoint { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Keyword { get; set; }

    public bool IsDesc { get; set; } = true;

    public bool IsError { get; set; }

    public string Env { get; set; }

    public int? LatMin { get; set; }

    public int? LatMax { get; set; }

    public bool HasPage { get; set; } = true;

    public string SpanId { get; set; }
}
