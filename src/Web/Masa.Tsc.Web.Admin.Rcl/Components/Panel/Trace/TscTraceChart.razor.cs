// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceChart
{    
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = 300;

    [Parameter]
    public RequestTraceListDto Query { get; set; } = default!;

    private object _options = new object();

    private void ConvertOption(string[] xPoints, IEnumerable<ChartViewDto> data)
    {
        _options = new
        {
            Legend = new
            {
                Data = data.Select(x => x.Title),
            },
            XAxis = new
            {
                Data = xPoints
            },
            YAxis = new { },
            Series =
            data.Select(item => new
            {
                Name = item.Title,
                Type = item.ChartType,
                Data = item.Data
            })


            //new[]
            //{
            //    new
            //    {
            //        Name= "Span个数",
            //        Type= "bar",
            //        Data= new []{ 5, 20, 3600, 100000, 10, 20 }
            //    },
            //    new
            //    {
            //        Name= "延迟时间",
            //        Type= "line",
            //        Data= new []{ 20, 20, 3006, 10, 10, 20 }
            //    }
            //}
        };
    }


    //public async Task LoadAsync()
    //{
    //    var data = await ApiCaller.TraceService.GetAggregateAsync(new RequestAggregationDto
    //    {
    //        End = Query.End,
    //        Start = Query.Start,
    //        FieldMaps = new RequestFieldAggregationDto[] {
    //            new RequestFieldAggregationDto{
    //                 AggegationType= Contracts.Admin.Enums.AggregationTypes.Count,
    //                 Name="span.id",
    //                 Alias="span_counts"
    //            }
    //        },
    //        Queries = ConvertToQueries(),
    //        Interval = GetInterval(),
    //    });

    //    var xPointes = data.List.Select(item => item.X).ToArray();

    //    var list = new List<ChartViewDto>()
    //    {
    //        new ChartViewDto{
    //            Title="Span Count",
    //            ChartType="bar",
    //            Pointes=data.List.Select(item=>item.Y).ToArray(),
    //        }
    //    };

    //    foreach (var item in data.List)
    //    {

    //    }

    //    await Task.CompletedTask;
    //}

    public async Task LoadAsync()
    {
        var data = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
        {
            End = Query.End,
            Start = Query.Start,
            FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= Contracts.Admin.Enums.AggregationTypes.DateHistogram,
                     Name="@timestamp",
                     Alias="Span Count",
                }
            },
            Queries = ConvertToQueries(isSpan: true),
            Interval = GetInterval(),
        });

        var data2 = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
        {
            End = Query.End,
            Start = Query.Start,
            FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.DateHistogram,
                     Name="@timestamp",
                     Alias="Trace Count",
                }
            },
            Queries = ConvertToQueries(isTrace: true),
            Interval = GetInterval(),
        });
        if (data.Data == null || !data.Data.Any())
            return;
        var xPoints = data.Data.Select(item => DateTime.Parse(item.X).Format(CurrentTimeZone, GetFormat())).ToArray();
        List<ChartViewDto> list = new()
        {
            new ChartViewDto
            {
                Title = "Span Count",
                ChartType = "bar",
                Data = data.Data.Select(item => item.Y).ToArray(),
            },
            new ChartViewDto
            {
                Title = "Trace Count",
                ChartType = "bar",
                Data = data2.Data.Select(item => item.Y).ToArray(),
            }
        };

        ConvertOption(xPoints, list);
        StateHasChanged();
        await Task.CompletedTask;
    }

    private string GetInterval()
    {
        var minites = (int)Math.Round((Query.End - Query.Start).TotalMinutes, 0);
        if (minites - 20 <= 0)
            return "1m";
        if (minites - 100 <= 0)
            return "5m";
        if (minites - 210 <= 0)
            return "15m";
        if (minites - 600 <= 0)
            return "30m";

        var hours = minites / 60;
        if (hours - 20 <= 0)
            return "1h";
        if (hours - 60 <= 0)
            return "3h";
        if (hours - 120 <= 0)
            return "6h";
        if (hours - 240 <= 0)
            return "12h";

        var days = hours / 24;
        if (days - 20 <= 0)
            return "1d";

        return "1month";
    }

    private string GetFormat()
    {
        var minites = (int)Math.Round((Query.End - Query.Start).TotalMinutes, 0);
        if (minites - 20 <= 0)
            return "HH:mm";
        if (minites - 100 <= 0)
            return "HH:mm";
        if (minites - 210 <= 0)
            return "HH:mm";
        if (minites - 600 <= 0)
            return "HH:mm";

        var hours = minites / 60;
        if (hours - 20 <= 0)
            return "dd H";
        if (hours - 60 <= 0)
            return "dd H";
        if (hours - 120 <= 0)
            return "dd H";
        if (hours - 240 <= 0)
            return "dd H";

        var days = hours / 24;
        if (days - 20 <= 0)
            return "MM-dd";

        return "yy-MM";
    }

    private Dictionary<string, string> ConvertToQueries(bool isSpan = false, bool isTrace = false)
    {
        var dic = new Dictionary<string, string>();
        if (Query.Service != null)
            dic.Add("service.name", Query.Service);
        if (Query.Instance != null)
            dic.Add("service.node", Query.Instance);
        if (Query.Endpoint != null)
            dic.Add("transaction.name", Query.Endpoint);

        dic.Add("isTrace", isTrace.ToString().ToLower());
        dic.Add("isSpan", isSpan.ToString().ToLower());

        return dic;
    }
}