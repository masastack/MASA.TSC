// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogsQuery(string Query, DateTime Start, DateTime End, int Page = 1, int Size = 10, string Sort = "desc", string Duration = "15m") : Query<PaginationDto<LogDto>>
{
    public override PaginationDto<LogDto> Result { get; set; }
}
