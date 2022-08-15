// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class GrowthChart
{
    private EChartPieOption _options = new()
    {
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

    protected override async Task LoadAsync(Dictionary<string,object> queryParams)
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
        _options.Series = new EChartOptionSerie[1] {
            new EChartOptionSerie{
                 Data=new List<EChartOptionSerieData>{
                   GetModel(true,data1),GetModel(false,data2)
                 }
            }
        };      
        await Task.CompletedTask;
    }

    private static EChartOptionSerieData GetModel(bool isTrace, string value)
    {
        return new EChartOptionSerieData { Name = isTrace ? "Tace" : "Log", Value = value };
    }
}
