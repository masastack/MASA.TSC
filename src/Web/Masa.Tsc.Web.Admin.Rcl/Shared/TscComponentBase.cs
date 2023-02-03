// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared;

public partial class TscComponentBase : BDomComponentBase
{
    [Inject]
    public IUserContext UserContext { get; set; }

    [Inject]
    public TokenProvider TokenProvider { get; set; }

    [Inject]
    public ILogger<TscComponentBase> Logger { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [CascadingParameter]
    public I18n I18n { get; set; }

    public string T(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        if (PageName is not null) return I18n.T(PageName, key, false) ?? I18n.T(key, false);
        else return I18n.T(key, true);
    }

    public string T(string formatkey, params string[] args)
    {
        return string.Format(T(formatkey), args);
    }

    protected virtual string? PageName { get; set; }

    public Guid CurrentUserId { get; private set; }

    public TimeZoneInfo CurrentTimeZone { get; set; } = TimeZoneInfo.Utc;

    protected virtual bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Loading = true;
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
        PopupService.AlertAsync(message, AlertTypes.Success);
    }

    public void OpenWarningMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Warning);
    }

    public void OpenErrorMessage(string message)
    {
        PopupService.AlertAsync(message, AlertTypes.Error);
    }

    //public static object GetDictionaryValue(object obj, string path)
    //{
    //    if (obj == null || string.IsNullOrEmpty(path))
    //        return default!;

    //    var keys = path.Split('.');
    //    foreach (var key in keys)
    //    {
    //        if (string.IsNullOrEmpty(key))
    //            continue;
    //        if (obj == null || obj is not Dictionary<string, object> dic)
    //        {
    //            return default!;
    //        }
    //        if (dic.ContainsKey(key))
    //        {
    //            obj = dic[key];
    //            continue;
    //        }

    //        var findKey = dic.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
    //        if (findKey != null)
    //        {
    //            obj = dic[findKey];
    //            continue;
    //        }
    //        return default!;
    //    }

    //    return obj;
    //}       
}