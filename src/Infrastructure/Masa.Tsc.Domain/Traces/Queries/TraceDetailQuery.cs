﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TraceDetailQuery(string TraceId, string Start, string End) : Query<IEnumerable<TraceResponseDto>>
{
    public override IEnumerable<TraceResponseDto> Result { get; set; }
}
