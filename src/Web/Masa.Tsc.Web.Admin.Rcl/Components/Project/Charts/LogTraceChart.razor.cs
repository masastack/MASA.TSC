// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class LogTraceChart
{

    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = 300;

    [Parameter]
    public bool Log { get; set; }

    [Parameter]
    public bool Trace { get; set; }

    [Parameter]
    public string Title { get; set; }

    private EChartLineOption _options = new()
    {
        Legend = new EChartOptionLegend
        {
            Data = new string[] { "Errors", "Caculates" },
            Left = true,
            Top = true,
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
        var data2 = new ChartLineDataDto<ChartPointDto>
        {
            Data = new ChartPointDto[] {
                new ChartPointDto{X="8-1",Y="15" },
                new ChartPointDto{X="8-2",Y="18" },
                new ChartPointDto{X="8-3",Y="81" },
                new ChartPointDto{X="8-4",Y="19" },
                new ChartPointDto{X="8-5",Y="23" },
                new ChartPointDto{X="8-6",Y="45" },
                new ChartPointDto{X="8-7",Y="32" }
            }
        };

        if (data1.Data == null || !data1.Data.Any())
            return;
        var xPoints = data1.Data.Select(item => DateTime.Parse(item.X).Format(CurrentTimeZone, GetFormat())).ToArray();

        //_options.Legend.Data= new string[] {"","" };
        _options.XAxis.Data = xPoints;
        _options.Series = new EChartLineOptionSerie[] {
            new EChartLineOptionSerie{
                Name="ERRORS",
                 Data=data1.Data.Select(item=>item.Y),
                  Type="line",
                  Stack="Total"
            },
            new EChartLineOptionSerie{
             Name="CACULATE",
                 Data=data2.Data.Select(item=>item.Y),
                  Type="line",
                  Stack="Total"
            }
        };

        //List<ChartViewDto> list = new()
        //{
        //    new ChartViewDto
        //    {
        //        Title = "Span Count",
        //        ChartType = "bar",
        //        Points = data1.Data.Select(item => item.Y).ToArray(),
        //    },
        //    new ChartViewDto
        //    {
        //        Title = "Trace Count",
        //        ChartType = "bar",
        //        Points = data2.Data.Select(item => item.Y).ToArray(),
        //    }
        //};

        //ConvertOption(xPoints, list);
        await Task.CompletedTask;
    }

    //private string GetInterval()
    //{
    //    var minites = (int)Math.Round((Query.End - Query.Start).TotalMinutes, 0);
    //    if (minites - 20 <= 0)
    //        return "1m";
    //    if (minites - 100 <= 0)
    //        return "5m";
    //    if (minites - 210 <= 0)
    //        return "15m";
    //    if (minites - 600 <= 0)
    //        return "30m";

    //    var hours = minites / 60;
    //    if (hours - 20 <= 0)
    //        return "1h";
    //    if (hours - 60 <= 0)
    //        return "3h";
    //    if (hours - 120 <= 0)
    //        return "6h";
    //    if (hours - 240 <= 0)
    //        return "12h";

    //    var days = hours / 24;
    //    if (days - 20 <= 0)
    //        return "1d";

    //    return "1month";
    //}

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

    //private Dictionary<string, string> ConvertToQueries(bool isSpan = false, bool isTrace = false)
    //{
    //    var dic = new Dictionary<string, string>();
    //    if (Query.Service != null)
    //        dic.Add("service.name", Query.Service);
    //    if (Query.Instance != null)
    //        dic.Add("service.node", Query.Instance);
    //    if (Query.Endpoint != null)
    //        dic.Add("transaction.name", Query.Endpoint);

    //    dic.Add("isTrace", isTrace.ToString().ToLower());
    //    dic.Add("isSpan", isSpan.ToString().ToLower());

    //    return dic;
    //}
}
