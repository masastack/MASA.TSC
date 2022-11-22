// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ErrorWarnChart
{
    [Parameter]
    public StringNumber Width { get; set; } = 180;

    [Parameter]
    public StringNumber Height { get; set; } = 190;

    [Parameter]
    public bool Error { get; set; }

    [Parameter]
    public bool Warn { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string SubText { get; set; } = "先写死\r\n从数据库读取加载";

    private EChartType _options = EChartConst.Pie;

    protected override void OnInitialized()
    {
        _options.SetValue("legend", new { buttom = "1%", right = true, orient = "horizontal" });
        _options.SetValue("series[0]", new
        {
            type = "pie",
            radius = "80%",
            emphasis = new
            {
                itemStyle = new
                {
                    shadowBlur = 10,
                    shadowOffsetX = 0,
                    shadowColor = "rgba(0, 0, 0, 0.5)"
                }
            },
            label = new { show = true }
        });
        base.OnInitialized();
    }    

    private int _total { get; set; }

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
                   Value=Warn ? "Warning" : "Error"
                } }
            });

        var data2 = await ApiCaller.TraceService.AggregateAsync<int>(
            new SimpleAggregateRequestDto
            {
                Start = start,
                End = end,
                Name= "@timestamp",
                Alias= "Count",
                Type=  AggregateTypes.Count,
                Service=query.AppId
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