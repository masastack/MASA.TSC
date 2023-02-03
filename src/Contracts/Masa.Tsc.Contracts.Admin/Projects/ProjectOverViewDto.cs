// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class ProjectOverviewDto : ProjectDto
{
    public bool HasError { get; set; }

    public bool HasWarning { get; set; }

    public MonitorStatuses Status { get; set; } = MonitorStatuses.Normal;
}