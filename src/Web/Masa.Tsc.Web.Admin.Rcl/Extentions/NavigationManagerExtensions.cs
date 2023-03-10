// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class NavigationManagerExtensions
{
    public static void NavigateToDashboardConfiguration(this NavigationManager navigationManager, string dashboardId, string? serviceName, string? instance = null, string? endpoint = null)
    {
        string url;
        if (!string.IsNullOrEmpty(instance))
            url = $"/dashboard/configuration/{dashboardId}/{System.Web.HttpUtility.UrlEncode(serviceName)}/{System.Web.HttpUtility.UrlEncode(instance)}";
        else if (!string.IsNullOrEmpty(endpoint))
            url = $"/dashboard/configuration/{dashboardId}/{System.Web.HttpUtility.UrlEncode(serviceName)}/endpoint/{System.Web.HttpUtility.UrlEncode(endpoint)}";
        else if (string.IsNullOrEmpty(serviceName))
            url = $"/dashboard/configuration/{System.Web.HttpUtility.UrlEncode(dashboardId)}";
        else
            url = $"/dashboard/configuration/{dashboardId}/{System.Web.HttpUtility.UrlEncode(serviceName)}";
        //if (serviceName is not null)
        //{
        //    bashUri += $"/{serviceName}";
        //}
        //if (instanceName is not null)
        //{
        //    bashUri += $"/{instanceName}";
        //}
        navigationManager.NavigateTo(url);
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

    public static void NavigateToConfigurationChart(this NavigationManager navigationManager, string panelId)
    {
        navigationManager.NavigateTo($"/dashboard/configuration/chart/{panelId}");
    }
}
