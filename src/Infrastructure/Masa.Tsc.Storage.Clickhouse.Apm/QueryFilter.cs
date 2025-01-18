// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Models.Request;

internal class QueryFilter
{
    // internal int[] GetErrorStatusCodes(params int[] defaultErrorStatus) => string.IsNullOrEmpty(StatusCodes) ? defaultErrorStatus : StatusCodes.Split(',').Select(s => Convert.ToInt32(s)).Where(num => num != 0).ToArray();

    internal bool? IsServer { get; set; } = true;

    internal bool? IsMetric { get; set; }

    internal bool? IsTrace { get; set; }

    internal bool? IsLog { get; set; }

    internal bool? IsError { get; set; }

    internal bool IsInstrument { get; set; } = false;
}
