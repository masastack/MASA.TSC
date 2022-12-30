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

    [Parameter]
    public string SubText { get; set; } = "先写死\r\n从数据库读取加载";

    private double _total;
    private EChartType _options = EChartConst.Pie;

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
            query.End = DateTime.Now;
        var results = await ApiCaller.MetricService.GetMultiRangeAsync(new Contracts.Admin.Metrics.RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
            "http_server_duration_count{http_status_code!~\"5[0-9]+\"}",
                "http_server_duration_count{}"
           },
            Start = query.End.Value.AddMinutes(-5),
            End = query.End.Value,
            Step = "5s"
        });

        double[] values = new double[2];
        var index = 0;
        foreach (var item in results)
        {
            if (item != null && item.ResultType == ResultTypes.Matrix && item.Result != null && item.Result.Any())
            {
                var first = (QueryResultMatrixRangeResponse)item.Result.First();
                values[index++] = Convert.ToDouble(first.Values.Last()[1]);
            }
        }

        if (values[1] == 0)
        {
            values[0] = 100;
            _total = 1;
        }
        else
        {
            values[0] = Math.Round(values[0] * 100 /  values[1], 4);
            values[1] = 100 - values[0];
            _total = values[0];
        }



        //var data1 = await ApiCaller.LogService.AggregateAsync<int>(
        //    new SimpleAggregateRequestDto
        //    {
        //        Start = start,
        //        End = end,
        //        Name = "@timestamp",
        //        Alias = "Count",
        //        Type = AggregateTypes.Count,
        //        Service = query.AppId,
        //        Conditions = new FieldConditionDto[] { new FieldConditionDto {
        //           Name="SeverityText",
        //           Value = "Error"
        //        } }
        //    });

        //var data2 = await ApiCaller.TraceService.AggregateAsync<int>(
        //    new SimpleAggregateRequestDto
        //    {
        //        Start = start,
        //        End = end,
        //        Name = "@timestamp",
        //        Alias = "Count",
        //        Type = AggregateTypes.Count,
        //        Service = query.AppId
        //    });
        //_total += data1;
        //_total += data2;

        _options.SetValue("series[0].data", new object[] {GetModel(true,values[0]),
            GetModel(false,values[1]) });
    }

    private static object GetModel(bool isTrace, double value)
    {
        return new { name = isTrace ? "Tace" : "Log", value = value };
    }
}