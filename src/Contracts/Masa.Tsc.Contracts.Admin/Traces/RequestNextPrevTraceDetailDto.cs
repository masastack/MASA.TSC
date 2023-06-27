// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Traces;

public class RequestNextPrevTraceDetailDto
{
    public string Service { get; set; }

    public DateTime Time { get; set; }

    public string Url { get; set; }

    public bool IsNext { get; set; }

    public string TraceId { get; set; }
}
