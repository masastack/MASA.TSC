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

    private int _page = 1;
    private int _pageSize = 10;

    private bool _loading;

    [CascadingParameter]
    ConfigurationRecord? ConfigurationRecord { get; set; }

    [Parameter]
    public bool PageMode { get; set; }

    [Parameter]
    public DateTime StartDateTime { get; set; }

    [Parameter]
    public DateTime EndDateTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CompontentSearchAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await CompontentSearchAsync();
    }

    private async Task Search((DateTime start, DateTime end) dateTimes)
    {
        StartDateTime = dateTimes.start;
        EndDateTime = dateTimes.end;

        await PageSearchAsync();
    }

    private async Task Search((string? service, string? instance, string? endpoint, string? traceId) query)
    {
        _service = query.service;
        _instance = query.instance;
        _endpoint = query.endpoint;
        _traceId = query.traceId;

        await PageSearchAsync();
    }

    private async Task Search((int page, int size) pagination)
    {
        _page = pagination.page;
        _pageSize = pagination.size;

        await PageSearchAsync();
    }

    async Task CompontentSearchAsync()
    {
        if (PageMode is false && ConfigurationRecord is not null)
        {
            if ((StartDateTime, EndDateTime) != (ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime))
            {
                StartDateTime = ConfigurationRecord.StartTime.UtcDateTime;
                EndDateTime = ConfigurationRecord.EndTime.UtcDateTime;
                await PageSearchAsync();
            }
        }
    }

    private async Task PageSearchAsync()
    {
        _loading = true;

        RequestTraceListDto query = new()
        {
            Service = _service!,
            Instance = _instance!,
            Endpoint = _endpoint!,
            TraceId = _traceId!,
            Start = StartDateTime,
            End = EndDateTime,
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
            Match = $"sum (increase(http_server_duration_count{text}[23s]))",
        });

        var durationResult = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
        {
            End = query.End,
            Start = query.Start,
            Step = interval,
            Match = $"sum  (increase(http_server_duration_sum{text}[23s]))/sum (increase(http_server_duration_count[23s]))",
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

            var timeSpan = (long)Math.Floor(Convert.ToDouble(item[0]) * 1000);
            var time = timeSpan.ToDateTime();
            values[index].Item1 = time.Format(CurrentTimeZone, fmt);
            //values[index].Item1 = timeSpan.ToString();
            index++;
        }
        _chartData = values;
    }

    private string GetInterval(RequestTraceListDto query)
    {
        var total = (long)Math.Floor((query.End - query.Start).TotalSeconds);
        var step = total / 250;
        if (step <= 0)
            step = 1;
        return $"{step}s";
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
            Start = StartDateTime,
            End = EndDateTime
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
            Start = StartDateTime,
            End = EndDateTime
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
            Start = StartDateTime,
            End = EndDateTime
        };

        return ApiCaller.TraceService.GetAttrValuesAsync(query);
    }
}
