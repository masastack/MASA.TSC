// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Teams.Events.Query;

public record TeamMonitorQuery : Query<TeamMonitorDto>
{
    public long StartTime { get; set; }

    public long EndTime { get; set; }

    public string Keyword { get; set; }

    public string ProjectId { get; set; }

    public Guid UserId { get; set; }

    public override TeamMonitorDto Result { get; set; }
}

