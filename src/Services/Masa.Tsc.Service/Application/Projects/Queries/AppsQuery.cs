// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Projects;

public record AppsQuery : Query<List<AppDto>>
{
    public string ProjectId { get; set; }

    public override List<AppDto> Result { get; set; }
}
