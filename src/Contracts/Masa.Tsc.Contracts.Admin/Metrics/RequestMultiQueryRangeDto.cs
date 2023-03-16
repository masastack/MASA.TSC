// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestMultiQueryRangeDto
{
    public string? Layer { get; set; }

    public string? Service { get; set; }

    public string? Instance { get; set; }

    public string? EndPoint { get; set; }

    public DateTime Start { get; set; } = DateTime.Now;

    public DateTime End { get; set; } = DateTime.Now;

    public string Step { get; set; }

    public List<string> MetricNames { get; set; }
}