// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class MonitorCard
{
    [Parameter]
    public AppMonitorDto Data
    {
        get { return _appMonitorDto; }
        set
        {
            if (value == null) return;
            _appMonitorDto = value;
            UpdateItems();
        }
    }

    [Parameter]
    public StringNumber Value { get; set; }

    [Parameter]
    public EventCallback<StringNumber> ValueChanged { get; set; }

    private List<AppMonitorViewDto> _items { get; set; } = new List<AppMonitorViewDto>
            {
                new AppMonitorViewDto
                {
                    Text="MONITORING",
                    Color="#7C4DFF",
                    Icon="mdi-chart-line-variant"
                },
                new AppMonitorViewDto
                {
                    Text="WARN",
                    Color="#FF7D00",
                    Icon="mdi-bell-ring"
                },
                new AppMonitorViewDto
                {
                    Text="ERROR",
                    Color="#FF5252",
                    Icon="mdi-bell"
                },
                new AppMonitorViewDto
                {
                    Text="NORMAL",
                    Color="#69F0AE",
                    Icon="mdi-shield",
                    IsShowApp=false
                }
            };
    private AppMonitorDto _appMonitorDto = new();

    private void UpdateItems()
    {
        _items[0].ServiceTotal = _appMonitorDto.ServiceTotal;
        _items[0].AppTotal = _appMonitorDto.AppTotal;        

        _items[1].ServiceTotal = _appMonitorDto.ServiceWarn;
        _items[1].AppTotal = _appMonitorDto.AppWarn;

        _items[2].ServiceTotal = _appMonitorDto.ServiceError;
        _items[2].AppTotal = _appMonitorDto.AppError;

        _items[3].ServiceTotal = _appMonitorDto.Normal;
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

