// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record MultiQuery(RequestMultiQueryDto Data) : Query<List<QueryResultDataResponse>>
{
    public override List<QueryResultDataResponse> Result { get; set; }
}
