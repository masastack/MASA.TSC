// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestMultiQueryDto
{
    public string? Layer { get; set; }

    public string? Service { get; set; }

    public string? Instance { get; set; }

    public string? EndPoint { get; set; }

    public List<string> Queries { get; set; }

    public DateTime Time { get; set; } = DateTime.Now;
}