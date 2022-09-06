// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

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
            Bottom = true,
            Right = true,
            Orient = EchartOrientTypes.horizontal
        },
        Series = new EChartOptionSerie[] {
             new EChartOptionSerie{
                 Type="pie",
                 Radius="80%",
                 Emphasis=new(),
                 Label=new EChartPieBorderRadiusOptionLable{
                    Show=false
                 }
             }
        }
    };

    public int Total { get; set; } = 23;

    protected override async Task LoadAsync(Dictionary<string, object> queryParams)
    {
        //var data = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
        //{
        //    End = Query.End,
        //    Start = Query.Start,
        //    FieldMaps = new RequestFieldAggregationDto[] {
        //        new RequestFieldAggregationDto{
        //             AggegationType= Contracts.Admin.Enums.AggregationTypes.DateHistogram,
        //             Name="@timestamp",
        //             Alias="Span Count",
        //        }
        //    },
        //    Queries = ConvertToQueries(isSpan: true),
        //    Interval = GetInterval(),
        //});

        //var data2 = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
        //{
        //    End = Query.End,
        //    Start = Query.Start,
        //    FieldMaps = new RequestFieldAggregationDto[] {
        //        new RequestFieldAggregationDto{
        //             AggegationType= AggregationTypes.DateHistogram,
        //             Name="@timestamp",
        //             Alias="Trace Count",
        //        }
        //    },
        //    Queries = ConvertToQueries(isTrace: true),
        //    Interval = GetInterval(),
        //});
        //if (data.Data == null || !data.Data.Any())
        //    return;
        string data1 = "23";
        string data2 = "34";
        _options.Series[0].Data = new List<EChartOptionSerieData>{
                   GetModel(true,data1),GetModel(false,data2)
        };
        var tt = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var text = JsonSerializer.Serialize(_options, tt);
        await Task.CompletedTask;
    }

    private static EChartOptionSerieData GetModel(bool isTrace, string value)
    {
        return new EChartOptionSerieData { Name = isTrace ? "Tace" : "Log", Value = value };
    }
}