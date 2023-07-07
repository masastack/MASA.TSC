// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces.Queries;

public record TraceErrorStatusQuery() : Query<int[]>
{
    public override int[] Result { get; set; }
}
