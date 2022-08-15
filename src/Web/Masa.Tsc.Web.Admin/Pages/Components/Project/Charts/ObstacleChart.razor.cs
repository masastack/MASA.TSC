// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class ObstacleChart : TscEChartBase
{
    private EChartPieBorderRadiusOption _options = new()
    {
        Legend = new EChartOptionLegend
        {
            Data = new string[] { "Errors", "Warns", "Other" }
        },
        Series = new EChartPieBorderRadiusOptionSerie[] {
            new EChartPieBorderRadiusOptionSerie{
                 AvoidLabelOverlap=true,
                Lable=new EChartPieBorderRadiusOptionLable{
                 Position="center"
                },
                 Emphasis=new EChartPieBorderRadiusOptionEmphasis{
                     Lable=new EChartPieBorderRadiusOptionEmphasisLable{
                         Show=true,
                          FontSize="36",
                           FontWeight="bold",
                            Formatter="{b}"
                     }
                 },
                  ItemStyle=new EChartPieBorderRadiusOptionSerieItemSyle
                  {
                     BorderColor="#fff",
                     BorderRadius=10,
                     BorderWidth=2
                  },
                  Type="pie",
                  Radius=new string[]{"40%","70%" }
            }
        }
    };

    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = 300;

    [Parameter]
    public string Title { get; set; }

    public int Total { get; set; } = 23;

    [Parameter]
    public string SubText { get; set; }

    protected override async Task LoadAsync(Dictionary<string, object> searchParams)
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
        string data3 = "21";
        _options.Series[0].Data = new EChartOptionSerieData[] {
            GetModel("Errors",data1),GetModel("Warns",data2),GetModel("Others",data3)
        };
        await Task.CompletedTask;
    }

    private EChartOptionSerieData GetModel(string name, string value)
    {
        return new EChartOptionSerieData { Name = name, Value = value };
    }
}