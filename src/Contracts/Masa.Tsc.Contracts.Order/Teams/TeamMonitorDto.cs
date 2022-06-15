// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TeamMonitorDto
{
    public AppMonitorDto Monitor { get; set; }

    public List<ProjectOverViewDto> Projects { get; set; }                   
}
