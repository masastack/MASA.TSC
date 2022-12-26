// Copyright (c) MASA Stack All rights reserved.
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

    DateTimeOffset StartTime { get; set; }

    DateTimeOffset EndTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Mode is not null)
        {
            return;
        }
        await GetPanelsAsync();
    }

    async Task GetPanelsAsync()
    {
        //todo get panel config
        Panels.Clear();
        Panels.Add(new());
        await Task.CompletedTask;
    }

    async Task OnSaveAsync()
    {
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(DashboardId), Panels.ToArray());
    }

    void AddPanel()
    {
        Panels.Insert(0, new());
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (StartTime, EndTime) = times;
    }

    async Task SaveAsync()
    {
        await Task.CompletedTask;
        OpenSuccessMessage(T("Save success"));
    }
}
