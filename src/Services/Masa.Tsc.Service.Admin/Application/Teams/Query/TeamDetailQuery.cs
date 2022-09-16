// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Teams;

public record TeamDetailQuery(Guid TeamId, string ProjectId) : Query<TeamDto>
{
    public override TeamDto Result { get; set; }
}