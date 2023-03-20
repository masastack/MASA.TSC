// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Extentions;

internal static class TimeExtentions
{
    private const int MIN_STEP = 30;

    /// <summary>
    /// prometheus step
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string Interval(this DateTime start, DateTime end, string defaultInterval = "1m")
    {
        if (start == DateTime.MaxValue || start == DateTime.MinValue || start == end)
            return string.Empty;
        var total = (long)Math.Floor((end - start).TotalSeconds);
        var step = GetInvervalSecond(defaultInterval);
        var maxStep = total / 250;
        if (maxStep - MIN_STEP < 0)
            maxStep = MIN_STEP;
        if (maxStep - step <= 0)
            maxStep = step;
        return $"{maxStep}s";
    }

    private static long GetInvervalSecond(string value)
    {
        var num = Convert.ToInt64(value[..(value.Length - 1)]);
        return value[^1] switch
        {
            's' => num,
            'm' => num * 60,
            'h' => num * 3600,
            'w' => num * 3600 * 7,
            'y' => num * 3600 * 365,
            _ => 0,
        };
    }

    /// <summary>
    /// echart line bar time format
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string Format(this DateTime start, DateTime end)
    {
        if (start == DateTime.MaxValue || start == DateTime.MinValue || start == end)
            return string.Empty;
        var timeSpan = end - start;

        if (timeSpan.TotalMinutes - 60 < 0)
            return "mm:ss";

        if (timeSpan.Hours - 24 <= 0)
            return "HH:mm:ss";

        return " dd HH:mm";
    }

    public static DateTimeOffset ToDateTimeOffset(this DateTime? time, TimeZoneInfo? timeZoneInfo)
    {
        if (time == null)
            return DateTimeOffset.MinValue;
        if (timeZoneInfo == null)
            timeZoneInfo = TimeZoneInfo.Utc;
        return new DateTimeOffset(ticks: time.Value.Ticks + timeZoneInfo.BaseUtcOffset.Ticks, offset: timeZoneInfo.BaseUtcOffset);
    }
}
