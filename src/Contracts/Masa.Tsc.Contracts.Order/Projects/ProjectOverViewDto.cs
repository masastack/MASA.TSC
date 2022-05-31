// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Projects;

public class ProjectOverViewDto
{
    public string Id { get; set; }

    public string Identity { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string LabelName { get; set; }

    public List<AppDto> Apps { get; set; }

    public MonitorStatuses Status { get; set; } = MonitorStatuses.Nomal;
}