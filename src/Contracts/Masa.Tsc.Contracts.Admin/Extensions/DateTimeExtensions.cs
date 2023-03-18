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
            return new DateTimeOffset(offset: timeZone.BaseUtcOffset, ticks: offset.Ticks + timeZone.BaseUtcOffset.Ticks).LocalDateTime;
        }
        return offset.LocalDateTime;
    }

    public static string Format(this DateTime time, TimeZoneInfo? timeZone, string fmt = "yyyy-MM-dd HH:mm:ss")
    {
        //if (timeZone is not null)
        //    time = TimeZoneInfo.ConvertTime(time, timeZone);
        if (time == DateTime.MinValue || time == DateTime.MaxValue)
            return "";
        return time.ToString(fmt);
    }
}
