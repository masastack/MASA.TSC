// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces.Queries;

public record TraceDetailNextQuery(string Service,string TraceId,DateTime Time,string Url,bool IsNext): Query<IEnumerable<TraceResponseDto>>
{
    public override IEnumerable<TraceResponseDto> Result { get; set; }
}
