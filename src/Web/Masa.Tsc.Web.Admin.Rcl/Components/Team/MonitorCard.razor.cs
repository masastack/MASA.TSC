﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
    public MonitorStatuses Value { get; set; }

    [Parameter]
    public EventCallback<MonitorStatuses> ValueChanged { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue<AppMonitorDto>(nameof(Data), out var data))
        {
            UpdateItems();
        }
        await base.SetParametersAsync(parameters);
    }

    private List<AppMonitorViewDto> _items;
    private AppMonitorDto _appMonitorDto = new();

    private void UpdateItems()
    {
        if (_items == null)
            _items = new List<AppMonitorViewDto>
            {
                new AppMonitorViewDto
                {
                    Text="MONITORING",
                    Color="#7C4DFF",
                    Icon="monitor",
                    Value=default
                },
                new AppMonitorViewDto
                {
                    Text="WARNS",
                    Color="#FF7D00",
                    Icon="warning",
                    Value=MonitorStatuses.Warn
                },
                new AppMonitorViewDto
                {
                    Text="ERRORS",
                    Color="#FF5252",
                    Icon="error",
                    Value=MonitorStatuses.Error
                },
                new AppMonitorViewDto
                {
                    Text="NORMAL",
                    Color="#69F0AE",
                    Icon="default",
                    IsShowApp=true,
                    Value=MonitorStatuses.Normal
                }
            };

        _items[0].ServiceTotal = _appMonitorDto.ServiceTotal;
        _items[0].AppTotal = _appMonitorDto.AppTotal;

        _items[1].ServiceTotal = _appMonitorDto.ServiceWarn;
        _items[1].AppTotal = _appMonitorDto.WarnCount;

        _items[2].ServiceTotal = _appMonitorDto.ServiceError;
        _items[2].AppTotal = _appMonitorDto.ErrorCount;

        _items[3].ServiceTotal = _appMonitorDto.Normal;
        _items[3].AppTotal = _appMonitorDto.NormalAppTotal;
    }

    private string ItemStyle(AppMonitorViewDto appMonitor)
    {
        if (Value == appMonitor.Value)
        {
            return $"border: 1px solid {appMonitor.Color};border-radius: 16px;min-width:170px";
        }
        return "border: 1px solid #E4E8F3;border-radius: 16px;min-width:170px";
    }

    private async Task ValueChangedAsync(AppMonitorViewDto value)
    {
        await ValueChanged.InvokeAsync(value.Value);
    }
}

