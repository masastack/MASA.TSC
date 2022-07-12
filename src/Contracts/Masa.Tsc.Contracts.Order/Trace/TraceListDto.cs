// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TraceListDto
{
    public string TraceId { get; set; }

    public string Service { get; set; }

    public string Endpoint { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    //unit ms
    public double Duration { get; set; }
}
