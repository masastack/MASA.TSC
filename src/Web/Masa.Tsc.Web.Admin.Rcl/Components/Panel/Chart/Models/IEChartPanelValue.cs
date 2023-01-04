// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public interface IEChartPanelValue : IPanelValue
{
    public EChartType EChartType { get; set; }

    public Tooltip Tooltip { get; set; }

    public Legend Legend { get; set; }

    public Toolbox Toolbox { get; set; }

    public Axis XAxis { get; set; }

    public Axis YAxis { get; set; }

    public object GetChartOption();

    public string GetChartKey();
}
