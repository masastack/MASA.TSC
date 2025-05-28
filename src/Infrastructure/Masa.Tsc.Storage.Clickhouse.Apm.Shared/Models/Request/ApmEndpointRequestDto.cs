// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models.Request;

public class ApmEndpointRequestDto : BaseApmRequestDto
{
    public string Endpoint { get; set; }

    public string StatusCode { get; set; }    

    public string Method { get; set; }
}
