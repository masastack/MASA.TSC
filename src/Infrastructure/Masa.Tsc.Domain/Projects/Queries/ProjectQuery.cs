﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record ProjectQuery(string ProjectId) : Query<ProjectDto>
{
    public override ProjectDto Result { get; set; }
}