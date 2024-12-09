﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TopologyQuery(TopologyRequestDto Data) : Query<TopologyResultDto>
{
    public override TopologyResultDto Result { get; set; }
}
