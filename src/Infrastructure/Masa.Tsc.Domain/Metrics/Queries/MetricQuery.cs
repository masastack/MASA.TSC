// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record MetricQuery(IEnumerable<string> Match) : Query<IEnumerable<string>>
{
    public override IEnumerable<string> Result { get; set; }
}
