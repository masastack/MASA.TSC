// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

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
            text.AppendLine($"{(hasWhere ? " ," : " (")}orderBy: {{{orderBy}}}");
        if (hasWhere || hasOrderBy)
            text.AppendLine(")");
        text.AppendLine("{");
        text.AppendLine(string.Join("\r\n", fields));
        text.AppendLine("}");
        text.AppendLine("}");
        text.AppendLine("}");
        Console.WriteLine(text.ToString());
        return text.ToString();
    }

    public static string GetEndpintListWhere(DateTime startUtc, DateTime endUtc, Guid teamId, string? env, string? service = default, string? endpoint = default, string? method = default, string? project = default)
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
        text.Append($",{CubejsConstants.TEAM_ID}:{{equals:\"{teamId}\"}}");
        if (!string.IsNullOrEmpty(project))
            text.Append($",{CubejsConstants.PROJECT}:{{equals:\"{project}\"}}");
        return text.ToString();
    }

    public static string GetEndpintListChartWhere(DateTime startUtc, DateTime endUtc, string? env, Guid teamId, string[] services, string[] endpoints, string[] methods)
    {
        var text = new StringBuilder();
        text.Append($"{CubejsConstants.TIMESTAMP_AGG}: {{inDateRange: [\"{startUtc}\",\"{endUtc}\"]}}");
        text.Append($",period:{{equals:\"{GetPeriod(startUtc, endUtc)}\"}}");
        if (!string.IsNullOrEmpty(env))
            text.Append($",{CubejsConstants.ENV_AGG}:{{equals:\"{env}\"}}");
        text.Append($",{CubejsConstants.TEAM_ID}:{{equals:\"{teamId}\"}}");
        if (services != null && services.Length > 0)
            text.Append($",{CubejsConstants.SERVICENAME}:{{in:[\"{string.Join("\",\"", services)}\"]}}");
        if (endpoints != null && endpoints.Length > 0)
            text.Append($",{CubejsConstants.TARGET}:{{in:[\"{string.Join("\",\"", endpoints)}\"]}}");
        if (methods != null && methods.Length > 0)
            text.Append($",{CubejsConstants.METHOD}:{{in:[\"{string.Join("\",\"", methods)}\"]}}");
        return text.ToString();
    }

    public static string GetEndpintListOrderBy(string? order, bool? isDesc)
    {
        var desc = isDesc ?? true ? "desc" : "asc";
        switch (order?.ToLower())
        {
            case "name":
                return $"{CubejsConstants.TARGET}:{desc},{CubejsConstants.METHOD}:{desc}";
            case "service":
                return $"{CubejsConstants.SERVICENAME}:{desc}";
            case "latency":
                return $"{CubejsConstants.LATENCY}:{desc}";
            case "throughput":
                return $"{CubejsConstants.THROUGHPUT}:{desc}";
            case "failed":
            default:
                return $"{CubejsConstants.FAILED}:{desc}";

        }
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