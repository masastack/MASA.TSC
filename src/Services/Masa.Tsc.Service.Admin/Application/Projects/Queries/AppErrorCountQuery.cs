// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Projects;

public record AppErrorCountQuery(string AppId, DateTime Start, DateTime End) : Query<long>
{
    public override long Result { get; set; }
}