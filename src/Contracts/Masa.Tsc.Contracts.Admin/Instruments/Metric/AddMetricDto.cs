// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddMetricDto
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string DisplayName { get; set; }

    public string Unit { get; set; }

    public int Sort { get; set; }
}
