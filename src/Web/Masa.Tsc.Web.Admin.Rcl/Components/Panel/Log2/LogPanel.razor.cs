// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Logs;
using System.Linq;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log2;

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
            GetLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
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

    List<LogDto> Logs { get; set; } = new();

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
            Query = Search
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);
        Logs = response.Result;
        Total = response.Total;
        GenOption(default);
        Loading = false;
    }

    void GenOption(object data)
    {
        // TODO: 解析data生成一下数据

        string[] xAxisData = { "12/2", "12/3", "12/4", "12/5", "12/6", "12/7", "12/8" };
        long[] durations = { 10, 20, 66, 32, 112, 121, 5 };
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
                data = new[] { "span", "duration" },
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
