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

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        if (parameters.TryGetValue<AppMonitorDto>(nameof(Data), out var data))
        {
            UpdateItems();
        }
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
                    Text=T("MONITORING"),
                    Color="#7C4DFF",
                    Icon="monitor",
                    Value=default
                },
                new AppMonitorViewDto
                {
                    Text=T("WARNS"),
                    Color="#FF7D00",
                    Icon="warning",
                    Value=MonitorStatuses.Warn
                },
                new AppMonitorViewDto
                {
                    Text=T("ERRORS"),
                    Color="#FF5252",
                    Icon="error",
                    Value=MonitorStatuses.Error
                },
                new AppMonitorViewDto
                {
                    Text=T("NORMAL"),
                    Color="#69F0AE",
                    Icon="default",
                    IsShowApp=false,
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
    }

    private string ItemStyle(AppMonitorViewDto appMonitor)
    {
        if (Value != null && Value.IsT0 && Value.AsT0 == appMonitor.Value.ToString())
        {
            return $"border: 1px solid {appMonitor.Color};border-radius: 16px;min-width:170px";
        }
        return "border: 1px solid #E4E8F3;border-radius: 16px;min-width:170px";
    }

    private async Task ValueChangedAsync(AppMonitorViewDto value)
    {
        Value = value.Value.ToString();
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }
}

