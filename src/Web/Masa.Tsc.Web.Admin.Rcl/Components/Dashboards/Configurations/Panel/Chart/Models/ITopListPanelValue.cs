// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart.Models;

public interface ITopListPanelValue : IPanelValue
{
    public string Color { get; set; }

    public string SystemIdentity { get; set; }

    public string Desc { get; set; }

    public int MaxCount { get; set; }

    public ChartTypes ChartType { get; set; }

    public List<TopListOption> GetTopListOption();

    public void SetTopListOption(string href);

    public Func<TopListOption, Task>? TopListOnclick { get; set; }
}
