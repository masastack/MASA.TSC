// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class LogTraceStatiscChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = 300;

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public bool Log { get; set; }

    [Parameter]
    public bool Trace { get; set; }

    [Parameter]
    public bool Error { get; set; }

    [Parameter]
    public bool Warn { get; set; }

    private EChartBarOption _options = new()
    {
        XAxis = new EChartOptionAxis
        {
            Type = "category"
        },
        YAxis = new EChartOptionAxis
        {
            Type = "value"
        },
        Series = new EChartOptionBarSeries[] {
            new EChartOptionBarSeries{
                Type="bar"
            }
        }
    };

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

        var data1 = new ChartLineDataDto<ChartPointDto>
        {
            Data = new ChartPointDto[] {
                new ChartPointDto{X="8-1",Y="30" },
                new ChartPointDto{X="8-2",Y="60" },
                new ChartPointDto{X="8-3",Y="80" },
                new ChartPointDto{X="8-4",Y="20" },
                new ChartPointDto{X="8-5",Y="34" },
                new ChartPointDto{X="8-6",Y="34" },
                new ChartPointDto{X="8-7",Y="25" }
            }
        };

        if (data1.Data == null || !data1.Data.Any())
            return;
        _options.XAxis.Data = data1.Data.Select(item => DateTime.Parse(item.X).Format(CurrentTimeZone, GetFormat())).ToArray();
        _options.Series[0].Data = data1.Data.Select(item => item.Y);
        await Task.CompletedTask;
    }

    private string GetFormat()
    {
        return "M-d";
        //var minites = (int)Math.Round((Query.End - Query.Start).TotalMinutes, 0);
        //if (minites - 20 <= 0)
        //    return "HH:mm";
        //if (minites - 100 <= 0)
        //    return "HH:mm";
        //if (minites - 210 <= 0)
        //    return "HH:mm";
        //if (minites - 600 <= 0)
        //    return "HH:mm";

        //var hours = minites / 60;
        //if (hours - 20 <= 0)
        //    return "dd H";
        //if (hours - 60 <= 0)
        //    return "dd H";
        //if (hours - 120 <= 0)
        //    return "dd H";
        //if (hours - 240 <= 0)
        //    return "dd H";

        //var days = hours / 24;
        //if (days - 20 <= 0)
        //    return "MM-dd";

        //return "yy-MM";
    }
}
