// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestMetricAggDto
{
    public string Match { get; set; }

    public IEnumerable<string> Labels { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
