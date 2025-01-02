﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record AppsQuery(string ProjectId) : Query<List<AppDto>>
{
    public override List<AppDto> Result { get; set; }
}