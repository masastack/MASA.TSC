// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Log;

public partial class LogPanel
{
    string? _search;
    int _page = 1;
    int _pageSize = 10;

    string Search
    {
        get => _search!;
        set
        {
            _search = value;
            TaskId = default!;
            Page = 1;
        }
    }

    [Parameter]
    public string Service { get; set; }

    [Parameter]
    public string LogLevel { get; set; }

    [Parameter]
    public ConfigurationRecord? ConfigurationRecord { get; set; }

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

    List<KeyValuePair<long, long>> ChartData { get; set; }

    MECharts? MECharts { get; set; }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnInitializedAsync()
    {
        if (StartTime.HasValue && StartTime.Value > DateTime.MinValue)
            await GetPageLogsAsync();

        if (!string.IsNullOrWhiteSpace(TaskId))
        {
            Search = $"{{\"term\":{{\"Attributes.TaskId.keyword\":\"{TaskId}\"}}}}";
        }
    }

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        StateHasChanged();
    }

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
        StartTime = times.start.UtcDateTime;
        EndTime = times.end.UtcDateTime;
        await GetPageLogsAsync();
    }

    async Task OnAutoUpdate((DateTimeOffset start, DateTimeOffset end) times)
    {
        await InvokeAsync(() => OnUpdate(times));
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
            Query = _search!
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);
        Logs = response.Result.Select(item => new LogModel(item.Timestamp, item.ExtensionData.ToDictionary(item => item.Key, item => new LogTree(item.Value)))).ToList();
        Total = response.Total;
        await GetChartData();
        Loading = false;
    }

    protected string ToDateTimeStr(long value, string format)
    {
        var utcTime = value.ToDateTime(CurrentTimeZone);
        return utcTime.Format(format);
    }

    async Task GetChartData()
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
        bool isRawQuery = (_search?.IndexOfAny(new char[] { '{', '}' }) ?? -1) >= 0;
        var result = await ApiCaller.LogService.AggregateAsync<List<KeyValuePair<long, long>>>(new SimpleAggregateRequestDto
        {
            Start = start,
            End = end,
            Name = "@timestamp",
            Type = AggregateTypes.DateHistogram,
            Interval = "5m",
            Keyword = isRawQuery ? string.Empty : _search!,
            RawQuery = isRawQuery ? _search! : string.Empty,
            Conditions = conditions
        });
        ChartData = result ?? new();
    }

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Search) && !string.IsNullOrEmpty(Service) && !string.IsNullOrEmpty(LogLevel))
        {
            Search = $"{{\"term\":{{\"Resource.service.name.keyword\":\"{Service}\"}}}},{{\"term\":{{\"SeverityText.keyword\": \"{LogLevel}\"}}}}";
        }
        base.OnInitialized();
    }

    private object FormatChartData()
    {
        DateTime end = EndTime ?? default, start = StartTime ?? default;
        var format = start.Format(end);
        string[] xAxisData = ChartData?.Select(item => ToDateTimeStr(item.Key, format))?.ToArray() ?? Array.Empty<string>();
        long[] durations = ChartData?.Select(item => item.Value)?.ToArray() ?? Array.Empty<long>();
        var subText = $"{start.UtcFormatLocal(CurrentTimeZone)}～{end.UtcFormatLocal(CurrentTimeZone)}";

        return new
        {
            title = new
            {
                right = 20,
                top = -14,
                subtext = subText,
            },
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
                data = new[] { I18n.Log("Log Count") },
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
                    type = "value"
                },
            },
            series = new[]
            {
                new
                {
                    name = I18n.Log("Log Count") ,
                    type = "bar",
                    yAxisIndex = 0,
                    data =(object)durations,
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
                }
            },
            grid = new
            {
                top = 20,
                left = 70,
                right = 20,
                bottom = 50
            }
        };
    }
}