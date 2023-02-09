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
    private bool _isDesc = true;
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

    private async Task Search((int page, int size, bool desc) pagination)
    {
        _page = pagination.page;
        _pageSize = pagination.size;
        _isDesc = pagination.desc;
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
            PageSize = _pageSize,
            IsDesc= _isDesc,
        };

        _queryResult = await ApiCaller.TraceService.GetListAsync(query);

        await GetChartDataAsync(query);

        _loading = false;
    }

    private async Task GetChartDataAsync(RequestTraceListDto query)
    {
        var interval = query.Start.Interval(query.End);

        var queryResult = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto { 
            Instance=query.Instance,
            ServiceName=query.Service,
            Start=query.Start,
            End=query.End,
            Step=interval,
            MetricNames=new List<string> { $"round(sum (increase(http_server_duration_count[{interval}])),1)", $"round(sum (increase(http_server_duration_sum[{interval}]))/sum (increase(http_server_duration_count[{interval}])),1)" }
        });

        var spanResult = queryResult[0];
        var durationResult = queryResult[1];

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
        var fmt = GetFormat();
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
            index++;
        }
        _chartData = values;
    }

    private string GetFormat()
    {
        return "yyyy-MM-dd HH:mm:ss";
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

    private Task<IEnumerable<string>> QueryEndpoints(string service, string instance, string key)
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
