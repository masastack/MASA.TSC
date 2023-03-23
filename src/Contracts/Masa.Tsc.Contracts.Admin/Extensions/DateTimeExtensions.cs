// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime time)
    {
        return new DateTimeOffset(time).ToUnixTimeSeconds();
    }

    public static DateTime ToDateTime(this long timestamp, TimeZoneInfo? timeZone = default)
    {
        DateTimeOffset offset;
        if (timestamp - 0x7ffffffff > 0)
            offset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        else
            offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        if (timeZone != null && timeZone.BaseUtcOffset.TotalSeconds > 0)
        {
            return new DateTimeOffset(offset: timeZone.BaseUtcOffset, ticks: offset.Ticks + timeZone.BaseUtcOffset.Ticks).DateTime;
        }
        return offset.DateTime;
    }

    public static string Format(this DateTime time, string fmt = "yyyy-MM-dd HH:mm:ss")
    {
        if (time == DateTime.MinValue || time == DateTime.MaxValue)
            return "";
        return time.ToString(fmt);
    }

    public static string UtcFormatLocal(this DateTime time, TimeZoneInfo timeZoneInfo, string fmt = "yyyy-MM-dd HH:mm:ss")
    {
        if (time == DateTime.MinValue || time == DateTime.MaxValue)
            return "";
        return (time.ToUniversalTime() + timeZoneInfo.BaseUtcOffset).ToString(fmt);
    }
}
