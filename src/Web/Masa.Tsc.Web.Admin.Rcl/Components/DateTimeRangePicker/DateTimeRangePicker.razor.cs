// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class DateTimeRangePicker
{
    [Parameter, EditorRequired]
    public DateTimeOffset StartDateTime { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset> StartDateTimeChanged { get; set; }

    [Parameter, EditorRequired]
    public DateTimeOffset EndDateTime { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset> EndDateTimeChanged { get; set; }

    [Parameter]
    public EventCallback OnConfirm { get; set; }

    [Parameter]
    public EventCallback<TimeZoneInfo> OnTimeZoneInfoChange { get; set; }

    [Parameter]
    public Func<DateOnly, DateOnly, bool> StartTimeLimit { get; set; }

    [Parameter]
    public Func<DateOnly, DateOnly, bool> EndTimeLimit { get; set; }

    private static ReadOnlyCollection<TimeZoneInfo> _systemTimeZones = TimeZoneInfo.GetSystemTimeZones();

    private TimeSpan _internalOffset;

    private bool _menuValue;

    private TimeSpan _offset;

    private DateTimeOffset _internalStartDateTime;
    private DateTimeOffset _internalEndDateTime;

    private DateOnly _internalStartDate;
    private DateOnly _internalEndDate;
    private TimeOnly _internalStartTime;
    private TimeOnly _internalEndTime;

    private bool _disableEnsureValidDateTimes;

    private TimeZoneInfo _timeZone;
    private DateTimeOffset? _lastStartDateTime;
    private DateTimeOffset? _lastEndDateTime;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        EnsureValidDateTimes();
    }

    private void EnsureValidDateTimes()
    {
        if (_disableEnsureValidDateTimes)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(StartDateTime);
        ArgumentNullException.ThrowIfNull(EndDateTime);

        if (StartDateTime == default)
        {
            throw new ArgumentException($"{nameof(StartDateTime)} can not be default");
        }

        if (EndDateTime == default)
        {
            throw new ArgumentException($"{nameof(EndDateTime)} can not be default");
        }

        if (StartDateTime > EndDateTime)
        {
            throw new ArgumentException($"{nameof(StartDateTime)} must be less than {nameof(EndDateTime)}");
        }

        if (StartDateTime.Offset != EndDateTime.Offset)
        {
            throw new ArgumentException($"Inconsistency of 'Offset' between {nameof(StartDateTime)} and {nameof(EndDateTime)}");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (StartDateTime != default)
            {
                UpdateInternalStartDateTime(StartDateTime);
            }

            if (EndDateTime != default)
            {
                UpdateInternalEndDateTime(EndDateTime);
            }
            _offset = StartDateTime.Offset;
            _internalOffset = _offset;
            _timeZone = GetSelectTimeZone();
            StateHasChanged();
        }
    }

    private void MenuValueChanged(bool val)
    {
        _menuValue = val;

        if (!val) return;

        UpdateInternalStartDateTime(StartDateTime);
        UpdateInternalEndDateTime(EndDateTime);
        _internalOffset = _offset;
    }

    private void UpdateTimeZone(TimeZoneInfo timeZoneInfo)
    {
        _internalEndDateTime = new DateTimeOffset(ticks: _internalEndDateTime.Ticks, offset: timeZoneInfo.BaseUtcOffset);
        _internalStartDateTime = new DateTimeOffset(ticks: _internalStartDateTime.Ticks, offset: timeZoneInfo.BaseUtcOffset);
    }

    private void UpdateInternalStartDateTime(DateTimeOffset val)
    {
        _internalStartDateTime = val.TryDeepClone();
        _internalStartDate = DateOnly.FromDateTime(_internalStartDateTime.DateTime);
        _internalStartTime = TimeOnly.FromDateTime(_internalStartDateTime.DateTime);
    }

    private void UpdateInternalEndDateTime(DateTimeOffset val)
    {
        _internalEndDateTime = val.TryDeepClone();
        _internalEndDate = DateOnly.FromDateTime(_internalEndDateTime.DateTime);
        _internalEndTime = TimeOnly.FromDateTime(_internalEndDateTime.DateTime);
    }

    private void InternalStartDateChanged(DateOnly val)
    {
        _lastStartDateTime = _internalStartDateTime;
        _internalStartDate = val;
        _internalStartDateTime = new DateTimeOffset(val.Year, val.Month, val.Day, _internalStartTime.Hour, _internalStartTime.Minute,
            _internalStartTime.Second, _internalOffset);
    }

    private void InternalEndDateChanged(DateOnly val)
    {
        _lastEndDateTime = _internalEndDateTime;
        _internalEndDate = val;
        _internalEndDateTime = new DateTimeOffset(val.Year, val.Month, val.Day, _internalEndTime.Hour, _internalEndTime.Minute,
            _internalEndTime.Second,
            _internalOffset);
    }

    private void InternalStartTimeChanged(TimeOnly val)
    {
        _lastStartDateTime = _internalStartDateTime;
        _internalStartTime = val;
        _internalStartDateTime = new DateTimeOffset(
            _internalStartDateTime.Date.Year,
            _internalStartDateTime.Date.Month,
            _internalStartDateTime.Date.Day,
            val.Hour,
            val.Minute,
            val.Second,
            _internalOffset
        );
    }

    private void InternalEndTimeChanged(TimeOnly val)
    {
        _lastEndDateTime = _internalEndDateTime;
        _internalEndTime = val;
        _internalEndDateTime = new DateTimeOffset(
            _internalEndDateTime.Date.Year,
            _internalEndDateTime.Date.Month,
            _internalEndDateTime.Date.Day,
            val.Hour,
            val.Minute,
            val.Second,
            _internalOffset
        );
    }

    private async Task HandleOnConfirm()
    {
        _disableEnsureValidDateTimes = true;

        if (StartDateTimeChanged.HasDelegate)
        {
            await StartDateTimeChanged.InvokeAsync(_internalStartDateTime);
        }
        else
        {
            StartDateTime = _internalStartDateTime;
        }

        if (EndDateTimeChanged.HasDelegate)
        {
            await EndDateTimeChanged.InvokeAsync(_internalEndDateTime);
        }
        else
        {
            EndDateTime = _internalEndDateTime;
        }

        if (OnTimeZoneInfoChange.HasDelegate && _timeZone != GetSelectTimeZone())
        {
            _timeZone = GetSelectTimeZone();
            await OnTimeZoneInfoChange.InvokeAsync(_timeZone);
        }

        _disableEnsureValidDateTimes = false;

        _menuValue = false;
        _offset = _internalOffset;

        if ((_lastStartDateTime != null && _lastStartDateTime.Value.UtcDateTime != _internalStartDateTime.UtcDateTime
            || _lastEndDateTime != null && _lastEndDateTime.Value.UtcDateTime != _internalEndDateTime.UtcDateTime
            ) && OnConfirm.HasDelegate)
        {
            _ = OnConfirm.InvokeAsync();
        }
    }

    private void OnInternalOffsetUpdated(TimeZoneInfo timeZoneInfo)
    {
        if (GetSelectTimeZone() != timeZoneInfo)
        {
            timeZoneInfo = GetSelectTimeZone();
            if (OnTimeZoneInfoChange.HasDelegate)
                _ = OnTimeZoneInfoChange.InvokeAsync(timeZoneInfo);
            UpdateInternalStartDateTime(_internalStartDateTime.ToOffset(timeZoneInfo.BaseUtcOffset));
            UpdateInternalEndDateTime(_internalEndDateTime.ToOffset(timeZoneInfo.BaseUtcOffset));
            UpdateTimeZone(timeZoneInfo);
        }
    }

    private void CancelClick()
    {
        if (GetSelectTimeZone() != _timeZone)
        {
            if (OnTimeZoneInfoChange.HasDelegate)
                _ = OnTimeZoneInfoChange.InvokeAsync(_timeZone);
            UpdateInternalStartDateTime(_internalStartDateTime.ToOffset(_timeZone.BaseUtcOffset));
            UpdateInternalEndDateTime(_internalEndDateTime.ToOffset(_timeZone.BaseUtcOffset));
            UpdateTimeZone(_timeZone);
        }
        _menuValue = false;
    }

    private static string FormatDateTime(DateTimeOffset dateTime)
    {
        var str = dateTime.ToString(CultureInfo.CurrentUICulture);
        var lastIndex = str.LastIndexOf(" ", StringComparison.Ordinal);
        return str[..lastIndex];
    }

    private TimeZoneInfo GetSelectTimeZone()
    {
        return _systemTimeZones.FirstOrDefault(timeZone => timeZone.BaseUtcOffset == _internalOffset)!;
    }
}
