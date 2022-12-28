// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Metrics;

namespace Masa.Tsc.Service.Admin.Application.Metrics.Queries;

public record MultiRangeQuery(RequestMultiQueryRangeDto Data): Query<List<QueryResultDataResponse>>
{
    public override List<QueryResultDataResponse> Result { get; set; }
}
