// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class ExemplarModel
{
    public IDictionary<string, object>? Labels { get; set; }

    public string? Value { get; set; }

    public float TimeStamp { get; set; }
}
