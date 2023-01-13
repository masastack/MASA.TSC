// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Data.Prometheus.Enums;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ErrorWarnChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private double _total;
    private EChartType _options = EChartConst.Pie;
    private double[] values = new double[2];

    protected override void OnInitialized()
    {
        _options.SetValue("legend", new { bottom = 10, left = "center" });
        _options.SetValue("series[0]", new
        {
            type = "pie",
            radius = "60%",
            emphasis = new
            {
                itemStyle = new
                {
                    shadowBlur = 10,
                    shadowOffsetX = 0,
                    shadowColor = "rgba(0, 0, 0, 0.5)"
                }
            },
            label = new { show = false }
        });
        base.OnInitialized();
    }


    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        _total = 0;
        if (query == null)
            return;
        if (query.End == null)
            query.End = DateTime.UtcNow;
        if (query.Start == null)
            query.Start = query.End.Value.AddDays(-1);

        var step = (long)Math.Floor((query.End.Value - query.Start.Value).TotalSeconds);
        var results = await ApiCaller.MetricService.GetMultiQueryAsync(new RequestMultiQueryDto
        {
            Queries = new List<string> {
            $"sum by(service_name) (increase(http_server_duration_count{{http_status_code!~\"5..\"[{step}s]))",
            $"sum by(service_name) (increase(http_server_duration_count[{step}s]))"
           },
            Time = query.End.Value,
        });
      
        var index = 0;
        foreach (var item in results)
        {
            if (item != null && item.ResultType == ResultTypes.Vector && item.Result != null && item.Result.Any())
            {
                var first = (QueryResultInstantVectorResponse)item.Result.First();
                values[index++] = Convert.ToDouble(first.Value[1]);
            }
        }

        if (values[1] == 0)
        {
            values[0] = 100;
            _total = 100;
        }
        else
        {
            values[0] = Math.Round(values[0] * 100 / values[1], 4);
            values[1] = 100 - values[0];
            _total = values[0];
        }
        _options.SetValue("tooltip.formatter", "{d}%");
        _options.SetValue("series[0].data", new object[] {GetModel(true,values[0]),
            GetModel(false,values[1]) });
    }

    private static object GetModel(bool isSuccess, double value)
    {
        return new { name = isSuccess ? "Success" : "Fail", value };
    }
}