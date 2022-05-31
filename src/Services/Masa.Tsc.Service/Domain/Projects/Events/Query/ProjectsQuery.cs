// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Projects.Events.Query;

public record ProjectsQuery : Query<List<ProjectDto>>
{
    public Guid UserId { get; set; }

    public override List<ProjectDto> Result { get; set; }
}