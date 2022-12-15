// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTrace
{
    private PaginatedListBase<TraceResponseDto> _queryResult;
    private ValueTuple<string, string, string>[] _chartData;

    private string? _service;
    private string? _instance;
    private string? _endpoint;
    private string? _traceId;
    private DateTime _startDateTime;
    private DateTime _endDateTime;

    private int _page = 1;
    private int _pageSize = 10;

    private bool _loading;

    private async Task Search((DateTime start, DateTime end) dateTimes)
    {
        _startDateTime = dateTimes.start;
        _endDateTime = dateTimes.end;

        await SearchAsync();
    }

    private async Task Search((string? service, string? instance, string? endpoint, string? traceId) query)
    {
        _service = query.service;
        _instance = query.instance;
        _endpoint = query.endpoint;
        _traceId = query.traceId;

        await SearchAsync();
    }

    private async Task Search((int page, int size) pagination)
    {
        _page = pagination.page;
        _pageSize = pagination.size;

        await SearchAsync();
    }

    private async Task SearchAsync()
    {
        _loading = true;

        RequestTraceListDto query = new()
        {
            Service = _service!,
            Instance = _instance!,
            Endpoint = _endpoint!,
            TraceId = _traceId!,
            Start = _startDateTime,
            End = _endDateTime,
            Page = _page,
            PageSize = _pageSize
        };

        _queryResult = await ApiCaller.TraceService.GetListAsync(query);

        await GetChartDataAsync(query);

        _loading = false;
    }

    private async Task GetChartDataAsync(RequestTraceListDto query)
    {
        var interval = GetInterval(query);

        string text = "";
        if (!string.IsNullOrEmpty(query.Service))
        {
            text += $",service_name=\"{query.Service}\"";
        }
        if (!string.IsNullOrEmpty(query.Instance))
        {
            text += $",instance=\"{query.Instance}\"";
        }
        if (!string.IsNullOrEmpty(text))
            text = $"{{{text[1..]}}}";

        var spanResult = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
        {
            End = query.End,
            Start = query.Start,
            Step = interval,
            Match = $"sum(http_client_duration_count{text})",
        });

        var durationResult = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
        {
            End = query.End,
            Start = query.Start,
            Step = interval,
            Match = $"avg(http_client_duration_sum{text}/http_client_duration_count{text})",
        });

        if (spanResult.Result!.Length == 0 && durationResult.Result!.Length == 0)
        {
            _chartData = Array.Empty<ValueTuple<string, string, string>>();
            return;
        }

        var spans = (QueryResultMatrixRangeResponse)spanResult.Result[0];
        var durations = (QueryResultMatrixRangeResponse)durationResult.Result![0];

        var spanArray = spans?.Values?.ToArray();
        var durationArray = durations?.Values?.ToArray();
        bool hasFirst = spanArray != null, hasSecond = durationArray != null;
        var currentArray = hasFirst ? spanArray! : durationArray!;

        ValueTuple<string, string, string>[] values = new (string, string, string)[currentArray.Length];
        var index = 0;
        var fmt = GetFormat(query);
        foreach (var item in currentArray!)
        {
            if (hasFirst)
            {
                values[index].Item2 = (string)item[1];
                if (hasSecond)
                    values[index].Item3 = (string)durationArray![index][1];
            }
            else
            {
                values[index].Item3 = (string)item[1];
            }

            var timeSpan = (long)Math.Floor((double)item[0] * 1000);
            var time = timeSpan.ToDateTime();
            values[index].Item1 = time.Format(CurrentTimeZone, fmt);
            //values[index].Item1 = timeSpan.ToString();
            index++;
        }
        _chartData = values;
    }

    private string GetInterval(RequestTraceListDto query)
    {
        var timeSpan = query.End - query.Start;
        var minites = (int)Math.Floor(timeSpan.TotalMinutes);
        if (minites - 20 <= 0)
            return "10s";
        if (minites - 60 <= 0)
            return "1m";
        if (minites - 600 <= 0)
            return "5m";

        var hours = (int)Math.Floor(timeSpan.TotalHours);
        if (hours - 72 <= 0)
            return "30m";

        var days = (int)Math.Floor(timeSpan.TotalDays);
        if (hours - 7 <= 0)
            return "1h";

        //if (hours - 120 <= 0)
        //    return "6h";
        //if (hours - 240 <= 0)
        //    return "12h";


        if (days - 30 <= 0)
            return "1d";

        return "1month";
    }

    private string GetFormat(RequestTraceListDto query)
    {
        return "yyyy-MM-dd HH:mm:ss";
        //var timeSpan = query.End - query.Start;
        //var minites = (int)Math.Floor(timeSpan.TotalMinutes);
        //if (minites - 20 <= 0)
        //    return "HH:mm";
        //if (minites - 100 <= 0)
        //    return "HH:mm";
        //if (minites - 210 <= 0)
        //    return "HH:mm";
        //if (minites - 600 <= 0)
        //    return "HH:mm";

        //var hours = (int)Math.Floor(timeSpan.TotalHours);
        //if (hours - 20 <= 0)
        //    return "dd H";
        //if (hours - 60 <= 0)
        //    return "dd H";
        //if (hours - 120 <= 0)
        //    return "dd H";
        //if (hours - 240 <= 0)
        //    return "dd H";

        //var days = (int)Math.Floor(timeSpan.TotalDays);
        //if (days - 20 <= 0)
        //    return "MM-dd";

        //return "yy-MM";
    }

    private Task<IEnumerable<string>> QueryServices(string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 2,
            Type = AggregateTypes.GroupBy,
            Service = string.Empty,
            Keyword = key,
            Start = _startDateTime,
            End = _endDateTime
        };

        return ApiCaller.TraceService.GetAttrValuesAsync(query);
    }

    private Task<IEnumerable<string>> QueryInstances(string service, string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 2,
            Type = AggregateTypes.GroupBy,
            Service = service,
            Keyword = key,
            Start = _startDateTime,
            End = _endDateTime
        };

        return ApiCaller.TraceService.GetAttrValuesAsync(query);
    }

    private Task<IEnumerable<string>> QueryEndpoints(string service,string instance, string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 2,
            Type = AggregateTypes.GroupBy,
            Service = service,
            Instance = instance,
            Keyword = key,
            Start = _startDateTime,
            End = _endDateTime
        };

        return ApiCaller.TraceService.GetAttrValuesAsync(query);
    }
}
