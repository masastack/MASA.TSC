// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Shared;

public class TscComponentBase : ComponentBase
{
    [Inject]
    public ILogger<TscComponentBase> Logger { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [Parameter]
    public SettingDto Setting { get; set; } = new()
    {
        IsEnable = false,
        Langauge = "zh-cn",
        TimeZone = (byte)8,
        UserId = Guid.Empty
    };

    public Guid CurrentUserId { get; set; }

    public TimeZoneInfo CurrentTimeZone { get; set; } = TimeZoneInfo.Utc;

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

    public string FormatDateTime(DateTime? time, string fmt = "yyyy-MM-dd HH:mm:ss", bool isConvertTimeZone = true)
    {
        if (!time.HasValue)
            return default!;

        if (isConvertTimeZone)
            time = TimeZoneInfo.ConvertTime(time.Value, CurrentTimeZone);
        return time.Value.ToString(fmt);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Logger.LogInformation("OnAfterRender");

        base.OnAfterRender(firstRender);
    }

    public static List<KeyValuePair<int, string>> TimeSeries { get { return _durations; } }
}