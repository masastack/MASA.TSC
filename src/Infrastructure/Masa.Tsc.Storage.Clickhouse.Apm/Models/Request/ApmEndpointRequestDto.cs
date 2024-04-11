// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Models.Request;

public class ApmEndpointRequestDto : BaseApmRequestDto
{
    public string Endpoint { get; set; }
}
