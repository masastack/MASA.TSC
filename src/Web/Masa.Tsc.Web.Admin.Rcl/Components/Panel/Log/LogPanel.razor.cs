﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log;

public partial class LogPanel
{
    string? _search;
    int _page = 1;
    int _pageSize = 10;
    object _option = new object();

    string Search
    {
        get => _search;
        set
        {
            _search = value;
            TaskId = default!;
            Page = 1;
        }
    }

    [CascadingParameter]
    ConfigurationRecord? ConfigurationRecord { get; set; }

    [Parameter]
    public string TaskId { get; set; }

    [Parameter]
    public DateTime? StartTime { get; set; }

    [Parameter]
    public DateTime? EndTime { get; set; }

    [Parameter]
    public bool PageMode { get; set; }

    int Page
    {
        get => _page;
        set
        {
            _page = value;
            GetPageLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value;
            GetPageLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    long Total { get; set; }

    List<LogModel> Logs { get; set; } = new();

    MECharts? MECharts { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await GetCompontentLogsAsync();
    }

    protected async Task ResizeEChartAsync()
    {
        if (MECharts is not null)
            await MECharts.Resize();
    }

    async Task OnUpdate((DateTimeOffset start, DateTimeOffset end) times)
    {
        StartTime = times.start.ToUniversalTime().UtcDateTime;
        EndTime = times.end.ToUniversalTime().UtcDateTime;
        await GetPageLogsAsync();
    }

    async Task OnAutoUpdate((DateTimeOffset start, DateTimeOffset end) times)
    {
        await OnUpdate(times);
        StateHasChanged();
    }

    async Task GetCompontentLogsAsync()
    {
        if (PageMode is false && ConfigurationRecord is not null)
        {
            if ((StartTime, EndTime) != (ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime))
            {
                StartTime = ConfigurationRecord.StartTime.UtcDateTime;
                EndTime = ConfigurationRecord.EndTime.UtcDateTime;
                await GetPageLogsAsync();
            }
        }
    }

    async Task GetPageLogsAsync()
    {
        DateTime end = EndTime ?? default, start = StartTime ?? default;
        if (!string.IsNullOrEmpty(TaskId))
        {
            end = DateTime.MinValue;
            start = DateTime.MinValue;
        }

        Loading = true;
        var query = new LogPageQueryDto
        {
            PageSize = _pageSize,
            Start = start,
            End = end,
            Page = Page,
            TaskId = TaskId,
            Query = _search!,
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);
        Logs = response.Result.Select(item => new LogModel(item.Timestamp, item.ExtensionData.ToDictionary(item => item.Key, item => new LogTree(item.Value)))).ToList();
        Total = response.Total;
        await GenOption();
        Loading = false;
    }

    protected string ToDateTimeStr(long value, string format)
    {
        return value.ToDateTime().Format(CurrentTimeZone, format);
    }

    async Task GenOption()
    {
        DateTime end = EndTime ?? default, start = StartTime ?? default;
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(TaskId))
        {
            end = DateTime.MinValue;
            start = DateTime.MinValue;
            conditions.Add(new FieldConditionDto
            {
                Name = "Attributes.TaskId.keyword",
                Type = ConditionTypes.Equal,
                Value = TaskId
            });
        }
        var result = await ApiCaller.LogService.AggregateAsync<List<KeyValuePair<long, long>>>(new SimpleAggregateRequestDto
        {
            Start = start,
            End = end,
            Name = "@timestamp",
            Type = AggregateTypes.DateHistogram,
            Interval = "5m",
            Keyword = _search!,
            Conditions = conditions
        });

        var format = start.Format(end);
        string[] xAxisData = result?.Select(item => ToDateTimeStr(item.Key, format))?.ToArray() ?? Array.Empty<string>();
        long[] durations = result?.Select(item => item.Value)?.ToArray() ?? Array.Empty<long>();

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
            xAxis = new
            {
                type = "category",
                data = xAxisData,
                axisPointer = new
                {
                    type = "shadow"
                }
            },
            yAxis = new
            {
                name = T("Total Count"),
                type = "value"
            },
            series = new[]
            {
                new
                {
                    type = "bar",
                    yAxisIndex = 0,
                    data = (object)durations,
                    itemStyle = new
                    {
                        color = "#4318FF"
                    },
                    smooth = false
                }
            },
            grid = new
            {
                top = 30,
                left = 80,
                right = 20,
                bottom = 40
            }
        };
    }
}
