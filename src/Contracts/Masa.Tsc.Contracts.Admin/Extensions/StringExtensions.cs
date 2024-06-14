// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text;

namespace System;

public static class StringExtensions
{
    private readonly static Dictionary<string, string> dicSpicalChars = new Dictionary<string, string>() { { ".", "x2E" } };

    public static bool IsRawQuery([NotNull] this string text, bool isElasticsearch = false, bool isClickhouse = false)
    {
        if (string.IsNullOrEmpty(text))
            return false;
        if (isElasticsearch)
        {
            return text.IndexOfAny(new char[] { '{', '}' }) - 1 >= 0;
        }
        if (isClickhouse)
        {
            var starts = new List<string> { "and ", "or " };
            var aaaa = new List<string> { "=", "<>", "!=", " like ", "not like" };
            return starts.Exists(item => text.StartsWith(item)) || aaaa.Exists(item => text.Contains(item));
        }

        return false;
    }

    public static DateTime ParseUTCTime(this string str)
    {
        if (DateTime.TryParse(str, out var time))
        {
            return new DateTimeOffset(time, TimeSpan.Zero).DateTime;
        }
        return DateTime.MinValue;
    }

    public static string ToSafeBlazorUrl([NotNull] this string url)
    {
        if (string.IsNullOrEmpty(url))
            return default!;
        var builder = new StringBuilder(url);
        foreach (var old in dicSpicalChars.Keys)
        {
            builder.Replace(old, dicSpicalChars[old]);
        }
        return builder.ToString();
    }

    public static string ToNomalBlazorUrl([NotNull] this string url)
    {
        if (string.IsNullOrEmpty(url))
            return default!;
        var builder = new StringBuilder(url);
        foreach (var old in dicSpicalChars.Keys)
        {
            builder.Replace(dicSpicalChars[old], old);
        }
        return builder.ToString();
    }
}