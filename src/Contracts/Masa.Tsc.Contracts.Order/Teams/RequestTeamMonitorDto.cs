// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestTeamMonitorDto: FromUri<RequestTeamMonitorDto>
{
    public long StartTime { get; set; }

    public long EndTime { get; set; }

    public string Keyword { get; set; }

    public string ProjectId { get; set; }

    public Guid UserId { get; set; }
}
