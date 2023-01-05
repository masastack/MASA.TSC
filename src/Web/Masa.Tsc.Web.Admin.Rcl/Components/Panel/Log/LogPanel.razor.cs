// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log;

public partial class LogPanel
{
    string? _search;
    int _page = 1;
    int _pageSize = 10;
    DateTime? _startTime;
    DateTime? _endTime;
    object _option;

    string Search
    {
        get => _search;
        set
        {
            _search = value;
            _page = 1;
            //GetLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    DateTime? StartTime { get; set; }

    DateTime? EndTime { get; set; }

    int Page
    {
        get => _page;
        set
        {
            _page = value;
            GetLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value;
            GetLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    long Total { get; set; }

    List<LogModel> Logs { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetLogsAsync();
    }

    async Task GetLogsAsync()
    {
        Loading = true;
        var query = new LogPageQueryDto
        {
            PageSize = _pageSize,
            Start = StartTime ?? default,
            End = EndTime ?? default,
            Page = Page,
            //Query = Search
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);
        Logs = response.Result.Select(item => new LogModel(item.Timestamp, item.ExtensionData.ToDictionary(item => item.Key, item => new LogTree(item.Value)))).ToList();
        Total = response.Total;
        await GenOption();
        Loading = false;
    }

    protected string ToDateTimeStr(long value)
    {
        return value.ToDateTime().Format(CurrentTimeZone);
    }

    async Task GenOption()
    {
        // TODO: 解析data生成一下数据

        DateTime end = EndTime ?? DateTime.UtcNow, start = StartTime ?? end.AddDays(-1);


        var result=await ApiCaller.LogService.AggregateAsync<List<KeyValuePair<long,long>>>(new SimpleAggregateRequestDto
        {
            Start = start,
            End = end,
            Name = "@timestamp", //ElasticConstant.Log.Timestamp,
            Type = AggregateTypes.DateHistogram,
            Interval = "5m",
        });

        string[] xAxisData = result?.Select(item => ToDateTimeStr(item.Key))?.ToArray()?? Array.Empty<string>();
        long[] durations = result?.Select(item => item.Value)?.ToArray()??Array.Empty<long>();
        int[] spans = { 110, 22, 323, 110, 210, 11, 11 };

        _option = new
        {
            tooltip = new
            {
                trigger = "axis",
                axisPointer = new
                {
                    type = "cross",
                    crossStyle = new { color = "#A18BFF66" }
                }
            },
            legend = new
            {
                data = new[] { "count" },
                bottom = true
            },
            xAxis = new[]
            {
                new
                {
                    type = "category",
                    data = xAxisData,
                    axisPointer = new
                    {
                        type = "shadow"
                    }
                },
            },
            yAxis = new[]
            {
                new
                {
                    type = "value",
                    name = "span",
                    axisLabel = new
                    {
                        formatter = "{value}"
                    }
                },
                //new
                //{
                //    type = "value",
                //    name = "duration",
                //    axisLabel = new
                //    {
                //        formatter = "{value} ms"
                //    }
                //},
            },
            series = new[]
            {
                new
                {
                    name = "span",
                    type = "bar",
                    yAxisIndex = 0,
                    data = (object)spans,
                    itemStyle = new
                    {
                        color = "#4318FF"
                    },
                    lineStyle = new
                    {
                        color = "",
                        type = ""
                    },
                    smooth = false
                },
                //new
                //{
                //    name = "duration",
                //    type = "line",
                //    yAxisIndex = 1,
                //    data = (object)durations,
                //    itemStyle = new
                //    {
                //        color = ""
                //    },
                //    lineStyle = new
                //    {
                //        color = "#A18BFF",
                //        type = "dashed"
                //    },
                //    smooth = true
                //}
            },
            Grid = new
            {
                x = 30,
                x2 = 0,
                y = 5,
                y2 = 40
            }
        };
    }
}
