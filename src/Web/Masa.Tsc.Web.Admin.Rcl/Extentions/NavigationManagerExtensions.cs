// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class NavigationManagerExtensions
{
    public static void NavigateToDashboardConfiguration(this NavigationManager navigationManager, string dashboardId, string? service = null, string? relation = null)
    {
        var uri = $"/dashboard/configuration/{dashboardId}";
        if (string.IsNullOrEmpty(service) is false) uri += $"/{service}";
        if (string.IsNullOrEmpty(relation) is false) uri += $"/{relation}";

        navigationManager.NavigateTo(uri);
    }

    public static void NavigateToDashboardConfigurationRecord(this NavigationManager navigationManager, string dashboardId, string? service = null, string? relation = null)
    {
        var uri = $"/dashboard/configuration/record/{dashboardId}";
        if (service is not null) uri += $"/{service}";
        if (string.IsNullOrEmpty(relation) is false) uri += $"/{relation}";

        navigationManager.NavigateTo(uri);
    }

    public static void NavigateToConfigurationChart(this NavigationManager navigationManager, string dashboardId, string panelId, string? serviceName = null)
    {
        var uri = $"/dashboard/configuration/chart/{dashboardId}";
        if (serviceName is not null)
        {
            uri += $"/{serviceName}";
        }
        navigationManager.NavigateTo($"{uri}/{panelId}");
    }

    public static void NavigateToConfigurationChart(this NavigationManager navigationManager, string panelId)
    {
        navigationManager.NavigateTo($"/dashboard/configuration/chart/{panelId}");
    }
}
