// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Models.Request;

public class BaseApmRequestDto : RequestPageBase
{
    public string? Env { get; set; }

    public ComparisonTypes? ComparisonType { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Period { get; set; }

    public string? Service { get; set; }

    public string? Queries { get; set; }

    public string? OrderField { get; set; }

    public bool? IsDesc { get; set; }

    public string StatusCodes { get; set; }

    internal int[] GetErrorStatusCodes() => string.IsNullOrEmpty(StatusCodes) ? Constants.DefaultErrorStatus : StatusCodes.Split(',').Select(s => Convert.ToInt32(s)).Where(num => num != 0).ToArray();

    internal bool? IsServer { get; set; } = true;

    internal bool? IsMetric { get; set; }

    internal bool? IsTrace { get; set; }

    internal bool? IsLog { get; set; }

    internal bool? IsError { get; set; }
}
