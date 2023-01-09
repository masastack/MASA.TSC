// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class NavigationManagerExtensions
{
    public static void NavigateToDashboardConfiguration(this NavigationManager navigationManager, string dashboardId, string? serviceName = null, string? instanceName = null)
    {
        var bashUri = $"/dashboard/configuration/{dashboardId}";
        if (serviceName is not null)
        {
            bashUri += $"/{serviceName}";
        }
        if (instanceName is not null)
        {
            bashUri += $"/{instanceName}";
        }
        navigationManager.NavigateTo(bashUri);
    }

    public static void NavigateToDashboardConfigurationRecord(this NavigationManager navigationManager, string dashboardId, string? serviceName = null)
    {
        var bashUri = $"/dashboard/configuration/record/{dashboardId}";
        if (serviceName is not null)
        {
            bashUri += $"/{serviceName}";
        }
        navigationManager.NavigateTo(bashUri);
    }

    public static void NavigateToConfigurationChart(this NavigationManager navigationManager, string dashboardId, string panelId, string? serviceName = null)
    {
        var bashUri = $"/dashboard/configuration/chart/{dashboardId}";
        if (serviceName is not null)
        {
            bashUri += $"/{serviceName}";
        }
        navigationManager.NavigateTo($"{bashUri}/{panelId}");
    }
}
