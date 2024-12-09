// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record RangeQuery(string Match, string Step, DateTime Start, DateTime End) : Query<QueryResultDataResponse>
{
    public override QueryResultDataResponse Result { get; set; }
}
