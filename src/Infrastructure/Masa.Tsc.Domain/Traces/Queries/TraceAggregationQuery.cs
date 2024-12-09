// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TraceAggregationQuery(SimpleAggregateRequestDto Data) : Query<object>
{
    public override object Result { get; set; }
}