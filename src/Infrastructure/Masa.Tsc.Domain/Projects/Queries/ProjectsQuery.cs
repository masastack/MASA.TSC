// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record ProjectsQuery(Guid UserId) : Query<List<ProjectDto>>
{
    public override List<ProjectDto> Result { get; set; }
}