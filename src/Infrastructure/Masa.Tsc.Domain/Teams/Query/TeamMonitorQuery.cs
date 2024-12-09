// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record TeamMonitorQuery(Guid UserId, string ProjectId,Guid TeamId ,DateTime StartTime, DateTime EndTime, string Keyword) : Query<TeamMonitorDto>
{
    public override TeamMonitorDto Result { get; set; }
}