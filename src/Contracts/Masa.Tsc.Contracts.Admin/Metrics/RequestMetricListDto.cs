// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestMetricListDto
{
    public string? Layer { get; set; }

    public string? Service { get; set; }

    public string? Instance { get; set; }

    public string? Endpoint { get; set; }

    public MetricValueTypes Type { get; set; }
}