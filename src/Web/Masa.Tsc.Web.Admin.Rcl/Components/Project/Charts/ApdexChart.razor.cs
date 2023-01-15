// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System;

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

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query.Start is null)
            query.Start = DateTime.UtcNow.AddDays(-1);
        if (query.End is null)
            query.End = DateTime.UtcNow;
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> { $"round((count(http_server_duration_bucket>1000 and http_server_duration_bucket<=4000)*0.5+count(http_server_duration_bucket<=1000))/count(http_server_duration_bucket),0.0001)" },
            Start = query.Start.Value,
            End = query.End.Value,
            ServiceName=query.AppId,
            Step = "5m"
        });
        if (_data[0] != null && _data[0].ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix && _data[0].Result != null && _data[0].Result.Any())
        {
            var seriesData = ((QueryResultMatrixRangeResponse)_data[0].Result.First()).Values.Select(items => (string)items[1]).ToArray();
            var timeSpans = ((QueryResultMatrixRangeResponse)_data[0].Result.First()).Values.Select(items => Convert.ToDouble(items[0])).ToArray();
            Total = seriesData.Last();
            _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value)));
            _options.SetValue("series[0].data", seriesData);
        }
        else
        {
            _options.SetValue("xAxis.data", Array.Empty<string>());
            _options.SetValue("series[0].data", Array.Empty<string>());
            Total = "0";
        }

    }
}