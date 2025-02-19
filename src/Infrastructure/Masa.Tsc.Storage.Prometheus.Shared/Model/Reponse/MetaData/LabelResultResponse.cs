// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class LabelResultResponse : ResultBaseResponse
{
    public IEnumerable<string>? Data { get; set; }
}
