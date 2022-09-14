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

    private readonly EChartPieOption _options = new()
    {
        Legend = new EChartOptionLegend
        {
            Bottom = "1%",
            Right = "true",
            Orient = EchartOrientTypes.horizontal
        },
        Series = new EChartOptionSerie[] {
             new EChartOptionSerie{
                 Type="pie",
                 Radius="80%",
                 Emphasis=new(),
                 Label=new EChartPieBorderRadiusOptionLabel{
                    Show=false
                 }
             }
        }       
    };

    private int _total { get; set; }

    protected override async Task LoadAsync(ProjectAppSearchModel query)
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

        var data1 = await ApiCaller.LogService.AggregateAsync(new RequestAggregationDto
        {
            Start = start,
            End = end,
            FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.Count,
                     Name="@timestamp",
                     Alias="Count",
                }
            },
            Queries = ConvertToLogQueries(query),
            Interval = string.Empty,
        });

        var data2 = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
        {
            Start = start,
            End = end,
            FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.Count,
                     Name="@timestamp",
                     Alias="Count",
                }
            },
            Queries = ConvertToTraceQueries(query,isTrace: true),
            Interval = string.Empty,
        });
        _total += Convert.ToInt32(data1.First().Value);
        _total += Convert.ToInt32(data2.Data.First().Y);
        _options.Series[0].Data = new List<EChartOptionSerieData>{
            GetModel(true,data1.First().Value),GetModel(false,data2.Data.First().Y)
        };
        await Task.CompletedTask;
    }

    private Dictionary<string, string> ConvertToLogQueries(ProjectAppSearchModel query)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("Resource.service.name", query.AppId);
        dic.Add("SeverityText", Warn ? "Warning" : "Error");
        //if (Query.Interval != null)
        //    dic.Add("transaction.name", Query.Interval);

        return dic;
    }

    private Dictionary<string, string> ConvertToTraceQueries(ProjectAppSearchModel query,bool isSpan = false, bool isTrace = false)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("service.name", query.AppId);
        //if (Query.Interval != null)
        //    dic.Add("transaction.name", Query.Interval);

        dic.Add("isTrace", isTrace.ToString().ToLower());
        dic.Add("isSpan", isSpan.ToString().ToLower());

        return dic;
    }

    private static EChartOptionSerieData GetModel(bool isTrace, string value)
    {
        return new EChartOptionSerieData { Name = isTrace ? "Tace" : "Log", Value = value };
    }
}