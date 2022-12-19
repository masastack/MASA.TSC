﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration
{
    [Inject]
    List<UpsertPanelDto> Panels { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    [Parameter]
    public string? Mode { get; set; }

    int AppId { get; set; }

    string Search { get; set; }

    bool IsEdit { get; set; } = true;

    DateTimeOffset StartTime { get; set; } = DateTimeOffset.Now.AddDays(-1);

    DateTimeOffset EndTime { get; set; } = DateTimeOffset.Now;

    protected override async Task OnInitializedAsync()
    {
        if (DashboardId is null)
        {
            return;
        }
        //todo get panel config
        if (Panels.Any() is false)
        {
            Panels.Add(new());
        }

        await Task.CompletedTask;
    }

    void AddPanel()
    {
        Panels.Insert(0, new());
    }
}
