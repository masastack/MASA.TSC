// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class PanelMetricDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Range { get; set; }

    public string Caculate { get; set; }

    public string Color { get; set; }

    public string Unit { get; set; }

    public int Sort { get; set; }
}
