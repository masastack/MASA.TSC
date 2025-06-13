// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Request;

internal static partial class CubeJsRequestUtils
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

    public static string GetEndpintListWhereByDetail(DateTime startUtc, DateTime endUtc, Guid teamId, string? env, string? appType = default, string? service = default, string? endpoint = default, string? method = default, string? project = default,
        string? statusCode = default, string? textField = default, string? textValue = default)
    {
        var text = new StringBuilder(AppendEnvService(startUtc, endUtc, env, service, endpoint, method));
        if (!string.IsNullOrEmpty(statusCode))
            text.Append($",{CubejsConstants.STATUS_CODE}:{{equals:\"{statusCode}\"}}");
        if (!string.IsNullOrEmpty(textField) && !string.IsNullOrEmpty(textValue))
        {
            if (textField == StorageConst.Current.TraceId)
                text.Append($",{CubejsConstants.TRACEID}:{{equals:\"{textValue}\"}}");
            else if (textField == StorageConst.Current.SpanId)
                text.Append($",{CubejsConstants.SPANID}:{{equals:\"{textValue}\"}}");
            else if (textField == StorageConst.Current.Trace.UserId)
                text.Append($",{CubejsConstants.USERID}:{{equals:\"{textValue}\"}}");
            else if (textField == StorageConst.Current.Trace.URLFull)
                text.Append($",{CubejsConstants.REQUEST_QUERY}:{{contains:\"{textValue}\"}}");
            else if (textField == StorageConst.Current.Trace.HttpRequestBody)
                text.Append($",{CubejsConstants.REQUEST_BODY}:{{contains:\"{textValue}\"}}");
        }
        return text.ToString();
    }

    public static string GetEndpintListWhere(DateTime startUtc, DateTime endUtc, Guid teamId, string? env, string? appType = default, string? service = default, string? endpoint = default, string? method = default, string? project = default, List<EndpointListItemByDetailResponse>? filters = default)
    {
        var text = new StringBuilder(AppendEnvService(startUtc, endUtc, env, service, endpoint, method, filters, true));
        text.Append(AppendTeamProject(teamId, project, appType));
        //text.Append(AppendDetailFilter(filters, env, service, endpoint, method));
        return text.ToString();
    }

    //private static string AppendDetailFilter(List<EndpointListItemByDetailResponse>? filters, string? env, string? service, string? endpoint, string? method)
    //{
    //    if (filters == null || filters.Count == 0) return default!;
    //    var text = new StringBuilder();
    //    foreach (var item in filters)
    //    {
    //        text.Append(",or:");
    //        text.Append($"{{{CubejsConstants.SERVICENAME}:{{equals:\"{item.ServiceName}\"}},{CubejsConstants.TARGET}:{{equals:\"{item.Target}\"}},{CubejsConstants.METHOD}:{{equals:\"{item.Method}\"}}}}");
    //    }
    //    text.Remove(0, 4).Append("}").Insert(0, ",and: {");
    //    return text.ToString();
    //}

    public static string AppendEnvService(DateTime startUtc, DateTime endUtc, string? env, string? service = default, string? endpoint = default, string? method = default, List<EndpointListItemByDetailResponse>? filters = default, bool hasPeriod = false)
    {
        (string[]? services, string[]? endpoints, string[]? methods) = (filters?.Select(item => item.ServiceName).ToArray(), filters?.Select(item => item.Target).ToArray(), filters?.Select(item => item.Method).ToArray());

        if (services != null && services.Length == 0)
            services = [string.Empty];
        if (endpoints != null && endpoints.Length == 0)
            endpoints = [string.Empty];
        if (methods != null && methods.Length == 0)
            methods = [string.Empty];

        var text = new StringBuilder();
        text.Append($"{CubejsConstants.TIMESTAMP_AGG}: {{inDateRange: [\"{startUtc}\",\"{endUtc}\"]}}");
        if (hasPeriod)
            text.Append($",period:{{equals:\"{GetPeriod(startUtc, endUtc)}\"}}");
        if (!string.IsNullOrEmpty(env))
            text.Append($",{CubejsConstants.ENV_AGG}:{{equals:\"{env}\"}}");
        if (!string.IsNullOrEmpty(service))
            text.Append($",{CubejsConstants.SERVICENAME}:{{equals:\"{service}\"}}");
        else if (services != null && services.Length > 0)
            text.Append($",{CubejsConstants.SERVICENAME}:{{in:[\"{string.Join("\",\"", services)}\"]}}");
        if (!string.IsNullOrEmpty(endpoint))
            text.Append($",{CubejsConstants.TARGET}:{{equals:\"{endpoint}\"}}");
        else if (endpoints != null && endpoints.Length > 0)
            text.Append($",{CubejsConstants.TARGET}:{{in:[\"{string.Join("\",\"", endpoints)}\"]}}");
        if (!string.IsNullOrEmpty(method))
            text.Append($",{CubejsConstants.METHOD}:{{equals:\"{method}\"}}");
        else if (methods != null && methods.Length > 0)
            text.Append($",{CubejsConstants.METHOD}:{{in:[\"{string.Join("\",\"", methods)}\"]}}");
        return text.ToString();
    }

    public static string AppendTeamProject(Guid teamId, string? project = default, string? appType = default)
    {
        var text = new StringBuilder();
        text.Append($",{CubejsConstants.TEAM_ID}:{{equals:\"{teamId}\"}}");
        if (!string.IsNullOrEmpty(project))
            text.Append($",{CubejsConstants.PROJECT}:{{equals:\"{project}\"}}");
        if (!string.IsNullOrEmpty(appType) && Enum.TryParse(appType, out AppTypes appTypeEnum))
            text.Append($",{CubejsConstants.APPTYPE}:{{equals:\"{(int)appTypeEnum}\"}}");
        return text.ToString();
    }

    public static string GetEndpintListChartWhere(DateTime startUtc, DateTime endUtc, string? env, Guid? teamId, string[] services, string[] endpoints, string[] methods)
    {
        var text = new StringBuilder();
        text.Append($"{CubejsConstants.TIMESTAMP_AGG}: {{inDateRange: [\"{startUtc}\",\"{endUtc}\"]}}");
        text.Append($",period:{{equals:\"{GetPeriod(startUtc, endUtc)}\"}}");
        if (!string.IsNullOrEmpty(env))
            text.Append($",{CubejsConstants.ENV_AGG}:{{equals:\"{env}\"}}");
        if (teamId.HasValue)
            text.Append($",{CubejsConstants.TEAM_ID}:{{equals:\"{teamId}\"}}");
        if (services != null && services.Length > 0)
            text.Append($",{CubejsConstants.SERVICENAME}:{{in:[\"{string.Join("\",\"", services)}\"]}}");
        if (endpoints != null && endpoints.Length > 0)
            text.Append($",{CubejsConstants.TARGET}:{{in:[\"{string.Join("\",\"", endpoints)}\"]}}");
        if (methods != null && methods.Length > 0)
            text.Append($",{CubejsConstants.METHOD}:{{in:[\"{string.Join("\",\"", methods)}\"]}}");
        return text.ToString();
    }

    public static string GetEndpintDetailChartWhere(DateTime startUtc, DateTime endUtc, string? env, string service, string endpoint, string method)
    {
        var text = new StringBuilder();
        text.Append($"{CubejsConstants.TIMESTAMP_AGG}: {{inDateRange: [\"{startUtc}\",\"{endUtc}\"]}}");
        text.Append($",period:{{equals:\"{GetPeriod(startUtc, endUtc)}\"}}");
        if (!string.IsNullOrEmpty(env))
            text.Append($",{CubejsConstants.ENV_AGG}:{{equals:\"{env}\"}}");       

        if (!string.IsNullOrEmpty(service))
            text.Append($",{CubejsConstants.SERVICENAME}:{{equals:\"{service}\"}}");
        if(!string.IsNullOrEmpty(endpoint))
            text.Append($",{CubejsConstants.TARGET}:{{equals:\"{endpoint}\"}}");
        if(!string.IsNullOrEmpty(method))
            text.Append($",{CubejsConstants.METHOD}:{{equals:\"{method}\"}}");       
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