// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TeamMonitorDto
{
    public AppMonitorDto Monitor { get; set; }

    public List<ProjectOverviewDto> Projects { get; set; }
}
