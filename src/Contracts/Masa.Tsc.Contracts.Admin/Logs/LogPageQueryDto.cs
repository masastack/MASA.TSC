﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class LogPageQueryDto : Pagination<LogPageQueryDto>
{
    public string Query { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public bool IsDesc { get; set; } = true;

    public string SortField { get; set; }

    public string Duration { get; set; }

    public string SpanId { get; set; }

    public string Env { get; set; }

    /// <summary>
    ///  scheduler job taskid
    /// </summary>
    public string TaskId { get; set; }

    public string Service { get; set; }

    public string LogLevel { get; set; }

    public bool IsLimitEnv { get; set; } = true;
}