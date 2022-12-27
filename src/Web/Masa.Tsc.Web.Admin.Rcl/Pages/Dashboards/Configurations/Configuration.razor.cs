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
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(DashboardId));
        //todo get panel config
        Panels.Clear();
        if (detail.Panels != null && detail.Panels.Any())
            Panels.AddRange(detail.Panels);
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
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(DashboardId), Panels.ToArray());
        OpenSuccessMessage(T("Save success"));
    }
}
