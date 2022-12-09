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

    public int Total { get; set; } = 23;

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
                  color = "#A9F387" // 0% 处的颜色
              },
              new
              {
                  offset = 1,
                  color = "#48D8BF" // 100% 处的颜色
              }
            },
                globalCoord = false // 缺省为 false
            },
            shadowColor = "rgba(72,216,191, 0.3)",
            shadowBlur = 10,
            shadowOffsetY = 20
        });
        _options.SetValue("series[0].smooth", true);
        await Task.CompletedTask;
    }

    private int GetRate(int lastValue, int value, int count, int max)
    {
        int width = 300; int height = 180;
        double x = width * 1.0 / count;
        var temp = value - lastValue;
        double y = temp * 1.0 / max * height;



        if (temp > 0)
            return (int)Math.Floor(y / x * 45);
        else
            return (int)Math.Floor(y / x * 45);
    }
}
