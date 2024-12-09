﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record LatestLogQuery(DateTime Start, DateTime End,string Service, string Query, bool IsDesc = true) : Query<LogResponseDto>
{
    public override LogResponseDto Result { get; set; }
}
