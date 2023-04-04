// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

public partial class TscComponentBase : BDomComponentBase, IAsyncDisposable
{
    [CascadingParameter(Name = "Culture")]
    private string Culture { get; set; } = null!;

    [Inject]
    public IUserContext UserContext { get; set; }

    [Inject]
    public TokenProvider TokenProvider { get; set; }

    [Inject]
    public JsInitVariables JsInitVariables { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [Inject]
    public I18n I18n { get; set; }

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
        return await PopupService.SimpleConfirmAsync(title, content, AlertTypes.Error);
    }

    public async Task<bool> OpenConfirmDialog(string title, string content, AlertTypes type)
    {
        return await PopupService.SimpleConfirmAsync(title, content, type);
    }

    public async Task OpenConfirmDialog(string messgae, Func<Task> callback, AlertTypes type = AlertTypes.Warning)
    {
        if (await PopupService.SimpleConfirmAsync(I18n.T("OperationConfirmation"), messgae, type)) await callback.Invoke();
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