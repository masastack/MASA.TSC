// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Teams;

public record TeamMonitorQuery(Guid UserId, string ProjectId, long StartTime, long EndTime, string Keyword) : Query<TeamMonitorDto>
{
    public override TeamMonitorDto Result { get; set; }
}