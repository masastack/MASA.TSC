// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Projects;

public record ProjectsQuery(Guid UserId) : Query<List<ProjectDto>>
{
    public override List<ProjectDto> Result { get; set; }
}