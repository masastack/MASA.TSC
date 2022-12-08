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

    public SettingDto Setting { get; set; } = new SettingDto();

    private static List<KeyValuePair<int, string>> _durations = new List<KeyValuePair<int, string>>
    {
        KeyValuePair.Create(5,"最近5分钟"),
        KeyValuePair.Create(15,"最近15分钟"),
        KeyValuePair.Create(30,"最近30分钟"),
        KeyValuePair.Create(45,"最近45分钟"),
        KeyValuePair.Create(60,"最近1小时"),
        KeyValuePair.Create(120,"最近2小时"),
        KeyValuePair.Create(360,"最近6小时"),
        KeyValuePair.Create(720,"最近12小时"),
        KeyValuePair.Create(24*60,"最近1天"),
        KeyValuePair.Create(48*60,"最近2天"),
        KeyValuePair.Create(7*24*60,"最近1周"),
    };

    public Guid CurrentUserId { get; private set; }

    public TimeZoneInfo CurrentTimeZone { get; private set; } = TimeZoneInfo.Local;

    protected virtual bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Loading = true;
        if (UserContext != null && !string.IsNullOrEmpty(UserContext.UserId))
            CurrentUserId = Guid.Parse(UserContext.UserId);
        CurrentTimeZone = GetTimeZone();

        //if (Setting == null)
        //{
        //    Setting = await ApiCaller.SettingService.GetAsync(CurrentUserId);
        //    if (string.IsNullOrEmpty(Setting.Language))
        //    {
        //        Setting = GetDefaultSetting(CurrentUserId);
        //    }
        //    else
        //    {
        //        //Setting.UserId = CurrentUserId;
        //    }
        //    CurrentTimeZone = GetTimeZone();
        //}
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


    private TimeZoneInfo GetTimeZone()
    {
        string id = $"{Setting.TimeZone}:{Setting.TimeZoneOffset}", name = $"timeZone:{Setting.TimeZone},{id},lang:{Setting.Language}";
        return TimeZoneInfo.CreateCustomTimeZone(id, new TimeSpan(Setting.TimeZone, Setting.TimeZoneOffset, 0), name, name);
    }

    public static object GetDictionaryValue(object obj, string path)
    {
        if (obj == null || string.IsNullOrEmpty(path))
            return default!;

        var keys = path.Split('.');
        foreach (var key in keys)
        {
            if (string.IsNullOrEmpty(key))
                continue;
            if (obj is null || obj is not Dictionary<string, object> dic)
            {
                return default!;
            }
            if (dic.ContainsKey(key))
            {
                obj = dic[key];
                continue;
            }

            var findKey = dic.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
            if (findKey != null)
            {
                obj = dic[findKey];
                continue;
            }
            return default!;
        }

        return obj;
    }    

    private static SettingDto GetDefaultSetting(Guid userId)
    {
        var timeOffset = TimeZoneInfo.Local.BaseUtcOffset;
        short timeZone = (short)timeOffset.Hours, minite = (short)timeOffset.Minutes;
        return new SettingDto
        {
            UserId = userId,
            TimeZone = timeZone,
            Language = GetLangByTimeZone(timeZone, minite),
            TimeZoneOffset = minite
        };
    }

    private static string GetLangByTimeZone(int timeZone, int minite)
    {
        switch (timeZone)
        {
            case 8:
                return "zh-cn";
            default:
                return "en-us";
        }
    }

    public static List<KeyValuePair<int, string>> TimeSeries { get { return _durations; } }
}