﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record InstrumentListQuery(Guid UserId, string Keyword, int Page, int Size) : Query<PaginatedListBase<InstrumentListDto>>
{
    public override PaginatedListBase<InstrumentListDto> Result { get; set; }
}