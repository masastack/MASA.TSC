// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record DirectoryQuery(Guid Id, Guid UserId) : Query<UpdateFolderDto>
{
    public override UpdateFolderDto Result { get; set; }
}
