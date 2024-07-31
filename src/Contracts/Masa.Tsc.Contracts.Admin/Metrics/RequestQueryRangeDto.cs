// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestQueryRangeDto
{
    public Guid Id { get; set; }

    public string Query { get; set; }

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime EndTime { get; set; } = DateTime.Now;

    public string Step { get; set; }
}