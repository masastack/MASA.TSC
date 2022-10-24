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

    //[Parameter]
    //public ProjectAppSearchModel Query { get { return _query; } set { _query = value; _isLoading = true; } }

    private EChartLineOption _options = new()
    {
        Legend = new EChartOptionLegend
        {
            Data = new string[] { "Errors", "Caculates" },
            Left = "true",
            Orient = EchartOrientTypes.horizontal
        },
        Grid = new EChartOptionGrid
        {
            Left = "1%",
            Right = "2%",
            Top = "20%",
            ContainLabel = true
        }
    };
    //private ProjectAppSearchModel _query;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query == null)
            return;
        DateTime start = DateTime.Now.Date;
        DateTime end = DateTime.Now;
        if (query.Start.HasValue)
            start = query.Start.Value;
        if (query.End.HasValue)
            end = query.End.Value;

        string interval = GetInterval(start, end);
        if (Trace)
        {
            var data1 = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
            {
                End = end,
                Start = start,
                FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.DateHistogram,
                     Name="@timestamp",
                     Alias="Count",
                }
            },
                Queries = ConvertToTraceQueries(query, isSpan: true),
                Interval = interval,
            });

            var data2 = await ApiCaller.TraceService.AggregateAsync(new RequestAggregationDto
            {
                End = end,
                Start = start,
                OrderField = "@timestamp",
                FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.Avg,
                     Name="@timestamp",
                     Alias="Count",
                }
            },
                Queries = ConvertToTraceQueries(query, isTrace: true),
                Interval = interval,
            });

            if (data1.Data == null || !data1.Data.Any())
                return;
            var xPoints = data1.Data.Select(item => DateTime.Parse(item.X).Format(CurrentTimeZone, GetFormat(start, end))).ToArray();

            //_options.Legend.Data= new string[] {"","" };
            _options.XAxis.Data = xPoints;
            _options.Series = new EChartLineOptionSerie[] {
            new EChartLineOptionSerie{
                Name="Errors",
                 Data=data1.Data.Select(item=>item.Y),
                  Type="line",
                  Stack="Total"
            },
            new EChartLineOptionSerie{
             Name="Caculates",
                 Data=data2.Data.Select(item=>item.Y),
                  Type="line",
                  Stack="Total"
            }
        };
        }
        else
        {
            var data1 = await ApiCaller.LogService.AggregateAsync(new RequestAggregationDto
            {
                End = end,
                Start = start,
                FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.DateHistogram,
                     Name="@timestamp",
                     Alias="Count",
                }
            },
                Queries = ConvertToLogQueries(query),
                Interval = interval,
            });

            var data2 = await ApiCaller.LogService.AggregateAsync(new RequestAggregationDto
            {
                End = end,
                Start = start,
                FieldMaps = new RequestFieldAggregationDto[] {
                new RequestFieldAggregationDto{
                     AggegationType= AggregationTypes.DistinctCount,
                     Name="@timestamp",
                     Alias="Trace Count",
                }
            },
                Queries = ConvertToLogQueries(query),
                Interval = interval,
            });

            if (data1 == null || !data1.Any())
                return;
            var xPoints = data1.Select(item => DateTime.Parse(item.Key).Format(CurrentTimeZone, GetFormat(start, end))).ToArray();

            _options.XAxis.Data = xPoints;
            _options.Series = new EChartLineOptionSerie[] {
            new EChartLineOptionSerie{
                Name="Errors",
                 Data=data1.Select(item=>item.Value),
                  Type="line",
                  Stack="Total"
            },
            new EChartLineOptionSerie{
             Name="Caculates",
                 Data=data2.Select(item=>item.Value),
                  Type="line",
                  Stack="Total"
            }
        };
        }
        await Task.CompletedTask;
    }

    private string GetInterval(DateTime start, DateTime end)
    {
        var minites = (int)Math.Round((end - start).TotalMinutes, 0);
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

    private string GetFormat(DateTime start, DateTime end)
    {
        //return "M-d";
        var minites = (int)Math.Round((end - start).TotalMinutes, 0);
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

    private Dictionary<string, string> ConvertToLogQueries(ProjectAppSearchModel query)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("service.name", query.AppId);
        return dic;
    }

    private Dictionary<string, string> ConvertToTraceQueries(ProjectAppSearchModel query, bool isSpan = false, bool isTrace = false)
    {
        var dic = new Dictionary<string, string>();
        if (query.AppId != null)
            dic.Add("service.name", query.AppId);

        dic.Add("isTrace", isTrace.ToString().ToLower());
        dic.Add("isSpan", isSpan.ToString().ToLower());

        return dic;
    }
}
