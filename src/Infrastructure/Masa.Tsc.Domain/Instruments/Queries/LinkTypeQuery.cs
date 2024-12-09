// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record LinkTypeQuery(string Layer, MetricValueTypes Type) : Query<LinkResultDto>
{
    public override LinkResultDto Result { get; set; }
}
