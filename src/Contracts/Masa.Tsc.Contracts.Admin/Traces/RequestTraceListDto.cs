// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestTraceListDto : Pagination<RequestTraceListDto>
{
    public string TraceId { get; set; }

    public string Service { get; set; }

    public string Instance { get; set; }

    public string Endpoint { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public bool IsDesc { get; set; } = true;
}
