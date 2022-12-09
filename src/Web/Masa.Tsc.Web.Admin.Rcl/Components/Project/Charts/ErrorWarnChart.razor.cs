// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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

    private int _total;
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
        DateTime start = DateTime.Now.Date;
        DateTime end = DateTime.Now;
        if (query.Start.HasValue)
            start = query.Start.Value;
        if (query.End.HasValue)
            end = query.End.Value;

        var data1 = await ApiCaller.LogService.AggregateAsync<int>(
            new SimpleAggregateRequestDto
            {
                Start = start,
                End = end,
                Name = "@timestamp",
                Alias = "Count",
                Type = AggregateTypes.Count,
                Service = query.AppId,
                Conditions = new FieldConditionDto[] { new FieldConditionDto {
                   Name="SeverityText",
                   Value = "Error"
                } }
            });

        var data2 = await ApiCaller.TraceService.AggregateAsync<int>(
            new SimpleAggregateRequestDto
            {
                Start = start,
                End = end,
                Name = "@timestamp",
                Alias = "Count",
                Type = AggregateTypes.Count,
                Service = query.AppId
            });
        _total += data1;
        _total += data2;

        _options.SetValue("series[0].data", new object[] {GetModel(true,data1),
            GetModel(false,data2) });
    }

    private static object GetModel(bool isTrace, int value)
    {
        return new { name = isTrace ? "Tace" : "Log", value = value };
    }
}