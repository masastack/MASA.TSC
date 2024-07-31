// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestPanelMetricDto
{
    public Guid InstrumentId { get; set; }

    public Guid PanelId { get; set; }

    public string ServiceName { get; set; }

    public string Instance { get; set; }

    public string EndPoint { get; set; }

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime EndTime { get; set; } = DateTime.Now;

    public string Step { get; set; }
}