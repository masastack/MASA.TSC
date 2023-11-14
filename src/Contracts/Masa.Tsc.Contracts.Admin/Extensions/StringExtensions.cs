// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace System;

public static class StringExtensions
{
    public static bool IsRawQuery([NotNull] this string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
        if (ElasticSearchConst.IsElasticSeach)
        {
            return text.IndexOfAny(new char[] { '{', '}' }) - 1 >= 0;
        }
        if (ClickHouseConst.IsClickhouse)
        {
            var starts = new List<string> { "and ", "or " };
            var aaaa = new List<string> { "=", "<>", "!=", " like ", "not like" };
            return starts.Exists(item => text.StartsWith(item)) || aaaa.Exists(item => text.Contains(item));
        }

        return false;
    }
}