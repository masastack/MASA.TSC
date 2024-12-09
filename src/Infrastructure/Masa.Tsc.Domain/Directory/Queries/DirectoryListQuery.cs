// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record DirectoryListQuery(Guid UserId, int Page, int PageSize, string Keyword, bool IsIncludeInstrument = true) : Query<PaginatedListBase<FolderDto>>
{
    public override PaginatedListBase<FolderDto> Result { get; set; }
}
