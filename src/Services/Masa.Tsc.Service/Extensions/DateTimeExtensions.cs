// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class DateTimeExtensions
{
    public static long ToUnixTimeSpan(this DateTime time)
    {
        return new DateTimeOffset(time).ToUnixTimeSeconds();
    }
}
