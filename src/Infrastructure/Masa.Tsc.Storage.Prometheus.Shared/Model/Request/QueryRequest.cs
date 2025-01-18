// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class QueryRequest
{
    public string? Query { get; set; }

    public string? Time { get; set; }

    public string? TimeOut { get; set; }
}
