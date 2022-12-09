// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class MonitorCard
{
    [Parameter]
    public List<AppMonitorViewDto> Data { get; set; } = new();

    [Parameter]
    public StringNumber Value { get; set; }

    [Parameter]
    public EventCallback<StringNumber> ValueChanged { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Data = new List<AppMonitorViewDto>
            {
                new AppMonitorViewDto
                {
                    Text="MONITORING",
                    Color="#7C4DFF",
                    Name="12/128",
                    Icon="mdi-chart-line-variant"
                },
                new AppMonitorViewDto
                {
                    Text="WARN",
                    Color="#FF6E40",
                    Name="128",
                    Icon="mdi-bell-ring"
                },
                new AppMonitorViewDto
                {
                    Text="ERROR",
                    Color="#FF5252",
                    Name="128",
                    Icon="mdi-bell"
                },
                new AppMonitorViewDto
                {
                    Text="NORMAL",
                    Color="#69F0AE",
                    Name="128",
                    Icon="mdi-shield"
                }
            };
            StateHasChanged();
        }
        base.OnAfterRender(firstRender);
    }

    private string ItemStyle(AppMonitorViewDto appMonitor, bool active)
    {
        if (active)
        {
            return $"border: 1px solid {appMonitor.Color};border-radius: 16px;";
        }
        return "border: 1px solid #E4E8F3;border-radius: 16px;";
    }
}

