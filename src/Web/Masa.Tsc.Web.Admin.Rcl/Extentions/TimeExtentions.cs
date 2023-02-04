// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

internal static class TimeExtentions
{
    /// <summary>
    /// prometheus step
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string Interval(this DateTime start, DateTime end)
    {
        if (start == DateTime.MaxValue || start == DateTime.MinValue || start == end)
            return string.Empty;
        var total = (long)Math.Floor((end - start).TotalSeconds);
        var step = total / 250;
        if (step - 5 < 0)
            step = 5;
        return $"{step}s";
    }
}
