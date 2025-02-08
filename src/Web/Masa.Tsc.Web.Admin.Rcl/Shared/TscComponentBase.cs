// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

public partial class TscComponentBase : MasaComponentBase, IAsyncDisposable
{
    [Inject]
    public IUserContext UserContext { get; set; }

    [Inject]
    public TokenProvider TokenProvider { get; set; }

    [Inject]
    public JsInitVariables JsInitVariables { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    public Guid CurrentUserId { get; private set; }

    private TimeZoneInfo _timeZoneInfo;

    public TimeZoneInfo CurrentTimeZone
    {
        get
        {
            if (_timeZoneInfo == null || _timeZoneInfo.BaseUtcOffset != JsInitVariables.TimezoneOffset)
                _timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("user custom timezone", JsInitVariables.TimezoneOffset, default, default);
            return _timeZoneInfo;
        }
    }

    protected virtual bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Loading = true;
        if (Math.Floor(JsInitVariables.TimezoneOffset.TotalMinutes) == 0)
            await JsInitVariables.SetTimezoneOffset();
        if (IsSubscribeTimeZoneChange)
            JsInitVariables.TimezoneOffsetChanged += OnTimeZoneInfoChanged;
        if (UserContext != null && !string.IsNullOrEmpty(UserContext.UserId))
            CurrentUserId = Guid.Parse(UserContext.UserId);
        Loading = false;        
        await base.OnInitializedAsync();
    }

    public async Task<bool> OpenConfirmDialog(string content)
    {
        return await PopupService.SimpleConfirmAsync(I18n.T("Operation confirmation"), content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content)
    {
        return await PopupService.SimpleConfirmAsync(title, content, AlertTypes.Warning);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content, AlertTypes type)
    {
        return await PopupService.SimpleConfirmAsync(title, content, type);
    }

    public void OpenSuccessMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Success);
    }

    protected bool _disposing = false;
    public virtual async ValueTask DisposeAsync()
    {
        if (!_disposing)
        {
            if (IsSubscribeTimeZoneChange)
            {
                JsInitVariables.TimezoneOffsetChanged -= OnTimeZoneInfoChanged;
            }
            _disposing = true;
        }
    }

    protected virtual Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        _timeZoneInfo = timeZoneInfo;
        return Task.CompletedTask;
    }

    private void OnTimeZoneInfoChanged()
    {
        _ = InvokeAsync(() => OnTimeZoneInfoChanged(CurrentTimeZone));
    }

    protected virtual bool IsSubscribeTimeZoneChange => false;
}