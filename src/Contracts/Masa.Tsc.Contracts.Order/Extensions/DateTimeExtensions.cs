// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class DateTimeExtensions
{
    public static long ToUnixTimeSpan(this DateTime time)
    {
        return new DateTimeOffset(time).ToUnixTimeSeconds();
    }

    public static DateTime ToDateTime(this long timestamp)
    {
        DateTimeOffset offset;
        if (timestamp - 0x7ffffffff > 0)
            offset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        else
            offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return offset.DateTime;
    }
}
