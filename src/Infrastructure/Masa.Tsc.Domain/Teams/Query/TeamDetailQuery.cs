// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TeamDetailQuery(Guid TeamId, string ProjectId) : Query<TeamDto>
{
    public override TeamDto Result { get; set; }
}