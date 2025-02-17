﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record LogErrorTypesQuery(string Service, DateTime Start, DateTime End) : Query<List<LogErrorDto>>
{
    public override List<LogErrorDto> Result { get; set; }
}
