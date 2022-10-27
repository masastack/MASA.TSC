// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartPanelMetricItemModel
{
    public Guid Id { get; set; }=Guid.NewGuid();

    public string Color { get; set; }

    public string Name { get; set; }

    public string Unit { get; set; }

    public string Caculate { get; set; }
}
