// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Queries;

public record InstrumentQuery(Guid UserId, string Keyword, int Page, int Size) : Query<PaginationDto<InstrumentListDto>>
{
    public override PaginationDto<InstrumentListDto> Result { get; set; }
}