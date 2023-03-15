// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ServiceCallChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private EChartType _options = EChartConst.Line;

    private List<QueryResultDataResponse>? _data;
    private DateTime StartTime = DateTime.UtcNow.AddDays(-1);
    private DateTime EndTime = DateTime.UtcNow;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _options.SetValue("series[0].smooth", true);
        _options.SetValue("series[0].showSymbol", false);
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query == null)
            return;       
        if (query.Start.HasValue)
            StartTime = query.Start.Value;
        if (query.End.HasValue)
            EndTime = query.End.Value;

        var step = StartTime.Interval(EndTime);
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"round(sum by(service_name)(increase(http_server_duration_count[1m])),0.01)"
            },
            Service = query.AppId,
            Start = StartTime,
            End = EndTime,
            Step = step
        });

        SetData();
    }

    private void SetData()
    {
        List<string> values = new();
        var timeSpans = new List<double>();

        if (_data[0] != null && _data[0].ResultType == ResultTypes.Matrix && _data[0].Result != null && _data[0].Result.Any())
        {
            timeSpans.AddRange(((QueryResultMatrixRangeResponse)_data[0].Result![0]).Values!.Select(values => Convert.ToDouble(values[0])));
            values = ((QueryResultMatrixRangeResponse)_data[0].Result![0])!.Values!.Select(values => values[1].ToString()!).ToList();
        }
        var format = StartTime.Format(EndTime);
        _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value, format)));
        _options.SetValue("series[0].data", values);
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        SetData();
        StateHasChanged();
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
    }
}
