// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class GrowthChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = 300;

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string SubText { get; set; }

    private EChartLineOption _options = new()
    {
        XAxis = new EChartOptionAxis
        {
            Type = "category",
            Show=false            
        },
        YAxis = new EChartOptionAxis
        {
            Type = "value",
            Show = false
        },
        Grid = new EChartOptionGrid
        { 
            Left="2%",
            Right="3%",
            Bottom="10%"
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

        var data = new int[] { 20, 60, 80, 50 };

        int rate = GetRate(data[data.Length - 2], data[data.Length - 1], data.Length, data.Max());
        _options.XAxis.Data = new string[] { "8-1", "8-2", "8-3", "8-4" };
        _options.Series = new EChartLineOptionSerie[1] {
            new EChartLineOptionSerie{
                Type="line",
                Data=new object[] {
                    new EChartLineDataSymbolOption {
                    Value=data[0].ToString()
                },
                 new EChartLineDataSymbolOption {
                    Value=data[1].ToString()
                },
                 new EChartLineDataSymbolOption {
                    Value=data[2].ToString()
                },
                 new EChartLineDataSymbolOption {
                    Value=data[3].ToString(),
                    Symbol="arrow",
                    SymbolSize=40,
                    SymbolRotate=rate
                }}
            }
        };
        //_options.Series = new EChartOptionSerie[1] {
        //    new EChartOptionSerie{
        //         Data=new List<EChartOptionSerieData>{
        //           GetModel(true,data1),GetModel(false,data2)
        //         }
        //    }
        //};
        var tt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var str = JsonSerializer.Serialize(_options, tt);
        await Task.CompletedTask;
    }

    private static EChartOptionSerieData GetModel(bool isTrace, string value)
    {
        return new EChartOptionSerieData { Name = isTrace ? "Tace" : "Log", Value = value };
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

        //(value-lastValue) / value*45

        //var a = Math.Tan(1);
        //var b = Math.Tanh(1);
        //var c= Math.Tan(45);
        //var d = Math.Tanh(45);

        //return 0;
        //double height = 100, width = 100;
        //if(lastValue-value>0)
        //    return Math.Tan((lastValue-value)/value)
    }
}
