// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;

public class TeamDetailConfigurationRecord : ConfigurationRecord
{
    public string ProjectId { get; set; }

    public Guid TeamId { get; set; }

    public int TeamProjectCount { get; set; }

    public int TeamServiceCount { get; set; }

    public TeamDetailConfigurationRecord(NavigationManager navigationManager) : base(navigationManager)
    {
        ModelType = ModelTypes.All;
    }

    public override void NavigateToConfiguration()
    {
        var uri = $"/teamDetail/configuration/{Service}";
        NavigationManager.NavigateTo(uri);
    }

    public override void NavigateToConfigurationRecord()
    {
        var uri = $"/teamDetail/configuration/record/{Service}";
        NavigationManager.NavigateTo(uri);
    }

    public override void NavigateToChartConfiguration()
    {
        ArgumentNullException.ThrowIfNull(PanelId);
        var uri = $"/teamDetail/configuration/chart/{PanelId}/{Service}";
        NavigationManager.NavigateTo(uri);
    }

    public override void Clear()
    {
        base.Clear();
        ModelType = ModelTypes.All;
    }
}
