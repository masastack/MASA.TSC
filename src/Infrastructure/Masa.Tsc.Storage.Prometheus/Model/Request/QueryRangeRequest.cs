// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class QueryRangeRequest
{
    public string? Query { get; set; }

    public string? Start { get; set; }

    public string? End { get; set; }

    public string? Step { get; set; }

    public string? TimeOut { get; set; }
}
