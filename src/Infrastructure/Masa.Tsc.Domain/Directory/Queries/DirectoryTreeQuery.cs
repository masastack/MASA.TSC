// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record DirectoryTreeQuery(Guid UserId, bool IsContainsInstrument) : Query<IEnumerable<DirectoryTreeDto>>
{
    public override IEnumerable<DirectoryTreeDto> Result { get; set; }
}
