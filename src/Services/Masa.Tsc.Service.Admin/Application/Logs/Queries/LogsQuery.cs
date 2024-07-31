// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogsQuery(string Query, DateTime Start, DateTime End, int Page = 1, int Size = 10, bool IsDesc = true,
    string? SortField = null,
    string Duration = "15m",
    string? JobTaskId = null,
    string? SpanId = null,
    string? Service = null,
    string? Env = null,
    string? LogLevel = null,
    bool IsLimitEnv = false
    ) : Query<PaginatedListBase<LogResponseDto>>
{
    public override PaginatedListBase<LogResponseDto> Result { get; set; }
}
