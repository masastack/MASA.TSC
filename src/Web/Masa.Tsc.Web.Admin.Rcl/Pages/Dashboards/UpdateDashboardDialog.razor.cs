﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

public partial class UpdateDashboardDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid DashboardId { get; set; }

    public MForm? Form { get; set; }

    private UpdateDashboardDto Dashboard { get; set; } = new();

    protected override string? PageName { get; set; } = "DashboardBlock";

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetDashboardDetailAsync();
        }
    }

    public async Task GetDashboardDetailAsync()
    {
        await Task.CompletedTask;
        Dashboard = new();
    }

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible is true)
        {
            Dashboard = new();
        }
    }

    public async Task UpdatetFolderAsync()
    {
        var success = Form!.Validate();
        if (success)
        {
            await Task.CompletedTask;
            OpenSuccessMessage(T("Update dashboard data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
        }
    }
}
