// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces.Queries;

public record TraceMetricToDetailQuery(string Service, string Url, DateTime Start, DateTime End) : Query<IEnumerable<TraceResponseDto>>
{
    public override IEnumerable<TraceResponseDto> Result { get; set; }
}