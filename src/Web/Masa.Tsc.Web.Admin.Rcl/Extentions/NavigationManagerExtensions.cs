﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

public static class NavigationManagerExtensions
{
    public static void NavigateToDashboardConfiguration(this NavigationManager navigationManager, string dashboardId, string? service = null, string? instance = null,string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/{dashboardId}";
        if (string.IsNullOrEmpty(service) is false) uri += $"/{service}";
        if (string.IsNullOrEmpty(instance) is false) uri += $"/{instance}";
        if (string.IsNullOrEmpty(endpoint) is false) uri += $"/{HttpUtility.UrlEncode(endpoint)}";

        navigationManager.NavigateTo(uri);
    }

    public static void NavigateToDashboardConfigurationRecord(this NavigationManager navigationManager, string dashboardId, string? service = null, string? instance = null, string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/record/{dashboardId}";
        if (string.IsNullOrEmpty(service) is false) uri += $"/{service}";
        if (string.IsNullOrEmpty(instance) is false) uri += $"/{instance}";
        if (string.IsNullOrEmpty(endpoint) is false) uri += $"/{HttpUtility.UrlEncode(endpoint)}";

        navigationManager.NavigateTo(uri);
    }

    public static void NavigateToConfigurationChart(this NavigationManager navigationManager, string panelId, string dashboardId, string? service = null, string? instance = null, string? endpoint = null)
    {
        var uri = $"/dashboard/configuration/chart/{panelId}/{dashboardId}";
        if (string.IsNullOrEmpty(service) is false) uri += $"/{service}";
        if (string.IsNullOrEmpty(instance) is false) uri += $"/{instance}";
        if (string.IsNullOrEmpty(endpoint) is false) uri += $"/{HttpUtility.UrlEncode(endpoint)}";

        navigationManager.NavigateTo(uri);
    }
}
