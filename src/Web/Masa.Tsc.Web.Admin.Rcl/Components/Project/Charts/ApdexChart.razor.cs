﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ApdexChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    public string Total { get; set; }

    private EChartType _options = EChartConst.Line;

    private List<QueryResultDataResponse>? _data;

    private DateTime StartTime;

    private DateTime EndTime;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var step = query.Start!.Value.Interval(query.End!.Value);
        string metric = $@"round((sum(rate(http_server_duration_bucket{{le=""250""}}[{step}])) by (service_name) + 
                                 sum(rate(http_server_duration_bucket{{le=""1000""}}[{step}])) by (service_name)
                               ) /2/sum(rate(http_server_duration_bucket{{le=""+Inf""}}[{step}])) by (service_name),0.0001)";
        StartTime = query.Start.Value;
        EndTime = query.End.Value;
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> { metric },
            Start = query.Start.Value,
            End = query.End.Value,
            ServiceName = query.AppId,
            Step = step
        });
        SetData();
    }

    private void SetData()
    {
        if (_data != null && _data[0] != null && _data[0].ResultType == ResultTypes.Matrix && _data[0].Result != null && _data[0].Result!.Any())
        {
            var seriesData = ((QueryResultMatrixRangeResponse)_data[0].Result!.First()).Values!.Select(items => (string)items[1]).ToArray();
            var timeSpans = ((QueryResultMatrixRangeResponse)_data[0].Result!.First()).Values!.Select(items => Convert.ToDouble(items[0])).ToArray();
            Total = seriesData.Last();
            var format = StartTime.Format(EndTime);
            _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value, format)));
            _options.SetValue("series[0].data", seriesData);
        }
        else
        {
            _options.SetValue("xAxis.data", Array.Empty<string>());
            _options.SetValue("series[0].data", Array.Empty<string>());
            Total = "0";
        }
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        SetData();
        StateHasChanged();
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
    }
}