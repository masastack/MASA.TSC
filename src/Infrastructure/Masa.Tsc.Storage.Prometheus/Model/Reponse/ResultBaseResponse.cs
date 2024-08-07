// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class ResultBaseResponse
{
    public ResultStatuses Status { get; set; }

    public string? Error { get; set; }

    public string? ErrorType { get; set; }

    public IEnumerable<string>? Warnings { get; set; }
}
