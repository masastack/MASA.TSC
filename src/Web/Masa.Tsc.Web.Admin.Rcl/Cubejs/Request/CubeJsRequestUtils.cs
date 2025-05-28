// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Request;

internal partial class CubeJsRequestUtils
{
    public static string GetCompleteCubejsQuery(string view, string? where = default, string? orderBy = default, int? page = default, int? pageSize = default, params string[] fields)
    {
        var text = new StringBuilder("query {");
        text.AppendLine("cube ");
        if (page.HasValue || pageSize.HasValue)
        {
            if (pageSize.HasValue)
            {
                text.Append('(');
                if (pageSize.Value - 1 < 0)
                    pageSize = 20;
                text.Append($"limit:{pageSize.Value}");
                if (page.HasValue)
                {
                    if (page.Value - 1 < 0)
                        page = 1;
                    var start = (page.Value - 1) * pageSize.Value;
                    text.Append($",offset:{start}");
                }
                text.Append(')');
            }
        }
        text.AppendLine("{");
        text.AppendLine($"{view}");
        bool hasWhere = !string.IsNullOrEmpty(where), hasOrderBy = !string.IsNullOrEmpty(orderBy);
        if (hasWhere)
            text.AppendLine($" (where: {{{where}}}");
        if (hasOrderBy)
            text.AppendLine($"{(hasWhere ? " (" : ", ")}orderBy: {{{orderBy}}}");
        if (hasWhere || hasOrderBy)
            text.AppendLine(")");
        text.AppendLine("{");
        text.AppendLine(string.Join("\r\n", fields));
        text.AppendLine("}");
        text.AppendLine("}");
        text.AppendLine("}");
        return text.ToString();
    }

    public static string GetEndpintListWhere(DateTime startUtc, DateTime endUtc, string? env, string? service = default, Guid? teamId = default, string? endpoint = default, string? method = default)
    {
        var text = new StringBuilder();
        text.Append($"{CubejsConstants.TIMESTAMP_AGG}: {{inDateRange: [\"{startUtc}\",\"{endUtc}\"]}}");
        text.Append($",period:{{equals:\"{GetPeriod(startUtc, endUtc)}\"}}");
        if (!string.IsNullOrEmpty(env))
            text.Append($",{CubejsConstants.ENV_AGG}:{{equals:\"{env}\"}}");
        if (!string.IsNullOrEmpty(service))
            text.Append($",{CubejsConstants.SERVICENAME}:{{equals:\"{service}\"}}");
        if (!string.IsNullOrEmpty(endpoint))
            text.Append($",{CubejsConstants.TARGET}:{{equals:\"{endpoint}\"}}");
        if (!string.IsNullOrEmpty(method))
            text.Append($",{CubejsConstants.METHOD}:{{equals:\"{method}\"}}");
        return text.ToString();
    }

    public static string GetEndpintListOrderBy(string order, bool isDesc)
    {
        return "";
    }

    private static string GetPeriod(DateTime start, DateTime end, string? period = default)
    {
        var reg = new Regex(@"/d+", default, TimeSpan.FromSeconds(5));
        if (string.IsNullOrEmpty(period) || !reg.IsMatch(period))
        {
            return GetDefaultPeriod(end - start).Replace(" ", "_");
        }
        var unit = reg.Replace(period, "").Trim().ToLower();
        var units = new List<string> { "year", "month", "week", "day", "hour", "minute", "second" };
        var find = units.Find(s => s.StartsWith(unit));
        if (string.IsNullOrEmpty(find))
            find = "minute";
        return $"{reg.Match(period).Result}_{find}";
    }

    private static string GetDefaultPeriod(TimeSpan timeSpan)
    {
        if ((int)timeSpan.TotalHours < 1)
        {
            return "1 minute";
        }

        var days = (int)timeSpan.TotalDays;
        if (days <= 0)
        {
            if ((int)timeSpan.TotalHours - 12 <= 0)
            {
                return "1 minute";
            }
            return "30 minute";
        }

        if (days - 7 <= 0)
        {
            return "1 hour";
        }

        if (days - 30 <= 0)
        {
            return "1 day";
        }

        if (days - 365 <= 0)
        {
            return "1 week";
        }

        return "1 month";
    }
}