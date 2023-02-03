// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestMultiQueryDto
{
    public string ServiceName { get; set; }

    public string Instance { get; set; }

    public string EndPoint { get; set; }

    public List<string> Queries { get; set; }

    public DateTime Time { get; set; } = DateTime.Now;
}