// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class LogPageQueryDto : Pagination<LogPageQueryDto>
{
    public string Query { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public bool IsDesc { get; set; } = true;

    public string Duration { get; set; }

    public string SpanId { get; set; }

    /// <summary>
    ///  scheduler job taskid
    /// </summary>
    public string TaskId { get; set; }
}