// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;

public class DashboardConfigurationRecord : ConfigurationRecord
{
    public string DashboardId { get; set; }

    public DashboardConfigurationRecord(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    public override void Clear()
    {
        DashboardId = "";
        base.Clear();
    }

    public override void NavigateToConfiguration()
    {
        NavigationManager.NavigateToDashboardConfiguration(DashboardId, Service, Instance, Endpoint);
    }

    public override void NavigateToConfigurationRecord()
    {
        NavigationManager.NavigateToDashboardConfigurationRecord(DashboardId, Service, Instance, Endpoint);
    }

    public override void NavigateToChartConfiguration()
    {
        ArgumentNullException.ThrowIfNull(PanelId);
        NavigationManager.NavigateToChartConfiguration(PanelId, DashboardId, Service, Instance, Endpoint);
    }
}
