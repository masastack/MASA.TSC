// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Cubejs.Apm.Request;

internal static class GraphQLRequestUtils
{
    public static GraphQLHttpRequest GetEndpointListRequest(BaseApmRequestDto request)
    {
        var where = GetEndpointListRequestWhere(request);
        var orderBy = GetEndpointListRequestOrderBy(request);
        (int skip, int size) = GetPageParam(request);
        var req = new GraphQLHttpRequest(
            $@"query {{
  cube (offset: {skip}, limit: {size}) {{
    metrics({where}{orderBy}) {{     
      servicename
      namespace
      target
      method
      latencyagg
      throughput
      failedagg
    }}
  }}
}}");

        return req;
    }

    public static GraphQLHttpRequest GetServiceListRequest(BaseApmRequestDto request)
    {
        var where = GetEndpointListRequestWhere(request);
        var orderBy = GetEndpointListRequestOrderBy(request, true);
        (int skip, int size) = GetPageParam(request);
        var req = new GraphQLHttpRequest(
            $@"query {{
  cube (offset: {skip}, limit: {size}) {{
    metrics({where}{orderBy}) {{     
      servicename
      namespace
      latencyagg
      throughput
      failedagg
    }}
  }}
}}");

        return req;
    }

    public static GraphQLHttpRequest GetEndpointListTotalRequest(BaseApmRequestDto request)
    {
        var where = GetEndpointListRequestWhere(request);
        (int skip, int size) = GetPageParam(request);
        var req = new GraphQLHttpRequest(
            $@"query {{
  cube {{
    metrics({where}) {{     
      dcnt
    }}
  }}
}}");
        return req;
    }

    public static GraphQLHttpRequest GetServiceChartRequest(BaseApmRequestDto request)
    {
        var where = GetEndpointListRequestWhere(request);
        (int skip, int size) = GetPageParam(request);
        var req = new GraphQLHttpRequest(
            $@"query {{
  cube {{
    {CubejsConstants.ServiceChartModelViewName}({where},orderBy:{{{CubejsConstants.ServiceName}:asc,{CubejsConstants.Timestamp}:asc}}) {{     
     {CubejsConstants.Timestamp}
    {{value}}
     {CubejsConstants.ServiceName}
     {CubejsConstants.Latency}
     {CubejsConstants.Throughput}
     {CubejsConstants.Failed}
     {CubejsConstants.P99}
     {CubejsConstants.P95}
    }}
  }}
}}");
        return req;
    }


    public static GraphQLHttpRequest GetEndpointChartRequest(ApmEndpointRequestDto request)
    {
        var where = GetEndpointListRequestWhere(request);
        (int skip, int size) = GetPageParam(request);
        var req = new GraphQLHttpRequest(
            $@"query {{
  cube {{
    {CubejsConstants.EndpointChartModelViewName}({where},orderBy:{{{CubejsConstants.ServiceName}:asc,{CubejsConstants.Endpoint}:asc,{CubejsConstants.Method}:asc,{CubejsConstants.Timestamp}:asc}}) {{     
     {CubejsConstants.Timestamp}
    {{value}}
     {CubejsConstants.Method}
     {CubejsConstants.Endpoint}
     {CubejsConstants.Latency}
     {CubejsConstants.Throughput}
     {CubejsConstants.Failed}
     {CubejsConstants.P99}
     {CubejsConstants.P95}
    }}
  }}
}}");
        return req;
    }



    private static string GetEndpointListRequestWhere(BaseApmRequestDto request)
    {
        StringBuilder text = new StringBuilder();
        if (request.Start > DateTime.MinValue && request.End > request.Start && request.End < DateTime.MaxValue)
        {
            text.Append($",datekey: {{ inDateRange:[ \"{request.Start}\",\"{request.End}\" ] }}");
        }
        if (request.AppIds != null && request.AppIds.Count > 0)
        {
            text.Append($",servicename: {{ in : [\"{string.Join("\",\"", request.AppIds)}\"]}}");
        }
        else if (!string.IsNullOrEmpty(request.Service))
        {
            text.Append($",servicename: {{ equals : \"{request.Service}\"}}");
        }
        if (!string.IsNullOrEmpty(request.Env))
        {
            text.Append($",namespace: {{ equals: \"{request.Env}\"}}");
        }

        if (request is ApmEndpointRequestDto endpointQuery)
        {
            if (!string.IsNullOrEmpty(endpointQuery.Method))
                text.Append($",method: {{ equals: \"{endpointQuery.Method}\"}}");

            if (!string.IsNullOrEmpty(endpointQuery.Endpoint))
                text.Append($",target: {{ equals: \"{endpointQuery.Endpoint}\"}}");

            //if (endpointQuery.StatusCode != null)
        }

        text.Append($",period: {{ equals: \"{GetPeriodTable(request)}\"}}");

        if (text.Length > 0)
            text.Remove(0, 1).Insert(0, "where: {").Append("}");

        return text.ToString();
    }

    private static string GetEndpointListRequestOrderBy(BaseApmRequestDto request, bool isService = false)
    {
        var list = new List<string>();

        switch (request.OrderField)
        {
            case nameof(EndpointListDto.Service):
                {
                    list.Add("servicename");
                }
                break;
            case nameof(EndpointListDto.Failed):
                {
                    list.Add("failedagg");
                }
                break;
            case nameof(EndpointListDto.Throughput):
                {
                    list.Add("throughput");
                }
                break;
            case nameof(EndpointListDto.Name):
                {
                    if (isService)
                    {
                        list.Add("servicename");
                    }
                    else
                    {
                        list.Add("target");
                        list.Add("method");
                    }
                }
                ;
                break;
            case nameof(EndpointListDto.Latency):
                {
                    list.Add("latencyagg");
                }
                break;
        }

        var text = new StringBuilder();
        var desc = request.IsDesc ?? false ? "desc" : "asc";
        if (list.Count > 0)
        {
            foreach (var item in list)
            {
                text.Append($",{item}:{desc}");
            }
            text.Insert(1, "orderBy: {").Append("}");

            return text.ToString();
        }
        return default!;
    }

    private static string GetPeriodTable(BaseApmRequestDto query)
    {
        return GetPeriod(query).Replace(' ', '_');
    }


    private static string GetPeriod(BaseApmRequestDto query)
    {
        var reg = new Regex(@"/d+", default, TimeSpan.FromSeconds(5));
        if (string.IsNullOrEmpty(query.Period) || !reg.IsMatch(query.Period))
        {
            return GetDefaultPeriod(query.End - query.Start);
        }
        var unit = reg.Replace(query.Period, "").Trim().ToLower();
        var units = new List<string> { "year", "month", "week", "day", "hour", "minute", "second" };
        var find = units.Find(s => s.StartsWith(unit));
        if (string.IsNullOrEmpty(find))
            find = "minute";
        return $"{reg.Match(query.Period).Result} {find}";
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

    private static ValueTuple<int, int> GetPageParam(BaseApmRequestDto request)
    {
        if (request.PageSize <= 0)
            request.PageSize = 20;
        if (request.Page <= 0)
            request.Page = 1;
        return ValueTuple.Create((request.Page - 1) * request.PageSize, request.PageSize);
    }
}