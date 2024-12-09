// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TraceErrorStatusQuery() : Query<int[]>
{
    public override int[] Result { get; set; }
}
