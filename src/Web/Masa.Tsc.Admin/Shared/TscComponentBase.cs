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
    public SettingDto Setting { get; set; }

    public Guid CurrentUserId { get; set; }

    public TimeZoneInfo CurrentTimeZone { get; set; } = TimeZoneInfo.Local;

    public string FormatDateTime(DateTime? time, string fmt="yyyy-MM-dd HH:mm:ss", bool isConvertTimeZone = true)
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
}