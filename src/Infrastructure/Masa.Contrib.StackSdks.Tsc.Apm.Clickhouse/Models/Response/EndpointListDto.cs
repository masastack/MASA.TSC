// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Response;

public class EndpointListDto: ServiceListDto
{
    public string Method { get; set; }

    public string Endpoint { get; set; }
}
