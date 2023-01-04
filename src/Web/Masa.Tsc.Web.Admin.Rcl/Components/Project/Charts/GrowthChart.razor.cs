// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class GrowthChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string SubText { get; set; }

    public double Total { get; set; }

    private EChartType _options = EChartConst.Line;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        _options.SetValue("grid.bottom", 20);
        _options.SetValue("xAxis.splitLine.show", false);
        _options.SetValue("xAxis.axisTick.show", false);
        _options.SetValue("xAxis.axisLine.show", false);
        _options.SetValue("xAxis.axisLabel.show", false);
        _options.SetValue("yAxis.splitLine.show", false);
        _options.SetValue("yAxis.axisTick.show", false);
        _options.SetValue("yAxis.axisLine.show", false);
        _options.SetValue("yAxis.axisLabel.show", false);
        _options.SetValue("yAxis.splitArea.show", false);
        _options.SetValue("series[0].lineStyle.normal", new
        {
            width = 8,
            color = new
            {
                type = "linear",

                colorStops = new List<object>
                {
              new
              {
                  offset = 0,
                  color = "rgb(255,236,236)" // 0% 处的颜色
              },
              new
              {
                  offset = 1,
                  color = "rgb(248,76,54)" // 100% 处的颜色
              }
            },
                globalCoord = false // 缺省为 false
            },
            shadowColor = "rgba(72,216,191, 0.3)",
            shadowBlur = 10,
            shadowOffsetY = 20
        });
        _options.SetValue("series[0].smooth", true);
        if (query.Start is null)
            query.Start = DateTime.Now.AddDays(-1);
        if (query.End is null)
            query.End = DateTime.Now;
        var data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> { "(count(http_server_duration_bucket>1000 and http_server_duration_bucket<=4000)*0.5+count(http_server_duration_bucket<1000))/count(http_server_duration_bucket)" },
            Start = query.Start.Value,
            End = query.End.Value,
            Step = "5m"
        });       
        if (data[0] != null && data[0].ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Matrix)
        {
            var seriesData = ((QueryResultMatrixRangeResponse)data[0].Result.First()).Values.Select(items => Convert.ToDouble(items[1])*100).ToArray();
            Total = seriesData.Last();
            _options.SetValue("series[0].data", seriesData);
        }
        else
        {
            _options.SetValue("series[0].data", new double[0]);
            Total = 0;
        }
    }
}