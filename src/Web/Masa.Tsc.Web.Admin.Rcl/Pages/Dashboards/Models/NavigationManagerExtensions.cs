// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;

public static class NavigationManagerExtensions
{
    public static void NavigateToDashboardConfiguration(this NavigationManager navigationManager, string dashboardId, string? service = null, string? instance = null, string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/{dashboardId}";
        navigationManager.NavigateTo(BindUrl(uri, service, instance, endpoint));
    }

    public static void NavigateToDashboardConfigurationRecord(this NavigationManager navigationManager, string dashboardId, string? service = null, string? instance = null, string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/record/{dashboardId}";
        navigationManager.NavigateTo(BindUrl(uri, service, instance, endpoint));
    }

    public static void NavigateToChartConfiguration(this NavigationManager navigationManager, string panelId, string dashboardId, string? service = null, string? instance = null, string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/chart/{panelId}/{dashboardId}";
        navigationManager.NavigateTo(BindUrl(uri, service, instance, endpoint));
    }

    static string BindUrl(string uri, string? service = null, string? instance = null, string? endpoint = null)
    {
        if (string.IsNullOrEmpty(service) is false) uri += $"/{service}";
        if (string.IsNullOrEmpty(instance) is false) uri += $"/{instance}";
        if (string.IsNullOrEmpty(instance) && string.IsNullOrEmpty(endpoint) is false)
        {
            uri += $"/all";
        }
        if (string.IsNullOrEmpty(endpoint) is false) uri += $"/{HttpUtility.UrlEncode(endpoint)}";

        return uri;
    }
}
