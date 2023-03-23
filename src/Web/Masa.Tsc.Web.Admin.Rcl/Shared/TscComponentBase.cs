// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

public partial class TscComponentBase : BDomComponentBase, IAsyncDisposable
{
    [Inject]
    public IUserContext UserContext { get; set; }

    [Inject]
    public TokenProvider TokenProvider { get; set; }

    [Inject]
    public ILogger<TscComponentBase> Logger { get; set; }

    [Inject]
    public JsInitVariables JsInitVariables { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [CascadingParameter]
    public I18n I18n { get; set; }

    public string T(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        if (PageName is not null) return (I18n?.T(PageName, key, false) ?? I18n?.T(key, false))!;
        else return I18n?.T(key, true)!;
    }

    public string T(string formatkey, params string[] args)
    {
        return string.Format(T(formatkey), args);
    }

    protected virtual string? PageName { get; set; }

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
        if (IsSubscribeTimeZoneChange)
            JsInitVariables.TimezoneOffsetChanged += OnTimeZoneInfoChanged;
        if (UserContext != null && !string.IsNullOrEmpty(UserContext.UserId))
            CurrentUserId = Guid.Parse(UserContext.UserId);
        Loading = false;

        await base.OnInitializedAsync();
    }

    public async Task<bool> OpenConfirmDialog(string content)
    {
        return await PopupService.ConfirmAsync(T("Operation confirmation"), content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content)
    {
        return await PopupService.ConfirmAsync(title, content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content, AlertTypes type)
    {
        return await PopupService.ConfirmAsync(title, content, type);
    }

    public void OpenSuccessMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Success);
    }

    public void OpenWarningMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Warning);
    }

    public void OpenErrorMessage(string message)
    {
        PopupService.EnqueueSnackbarAsync(message, AlertTypes.Error);
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