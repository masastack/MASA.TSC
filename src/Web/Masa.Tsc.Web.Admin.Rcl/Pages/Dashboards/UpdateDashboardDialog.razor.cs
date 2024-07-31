// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

public partial class UpdateDashboardDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<UpdateDashboardDto> OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid DashboardId { get; set; }

    public MForm? Form { get; set; }

    private UpdateDashboardDto Dashboard { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetDashboardDetailAsync();
        }
    }

    public async Task GetDashboardDetailAsync()
    {
        Dashboard = await ApiCaller.InstrumentService.GetAsync(DashboardId);
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

    public async Task UpdatetDashboardAsync()
    {
        var success = Form!.Validate();
        if (success)
        {
            await ApiCaller.InstrumentService.UpdateAsync(Dashboard);
            OpenSuccessMessage(I18n.Dashboard("Update dashboard data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync(Dashboard);
        }
    }
}
