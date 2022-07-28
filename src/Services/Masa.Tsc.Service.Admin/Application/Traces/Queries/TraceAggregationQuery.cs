// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public record TraceAggregationQuery(bool IsTrace, bool IsSpan, IEnumerable<RequestFieldAggregationDto> Fields, Dictionary<string, string> Queries, DateTime Start, DateTime End, string Interval) : Query<ChartLineDataDto<ChartPointDto>>
{
    public override ChartLineDataDto<ChartPointDto> Result { get; set; }
}
