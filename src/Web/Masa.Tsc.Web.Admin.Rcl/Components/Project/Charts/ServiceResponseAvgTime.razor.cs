// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ServiceResponseAvgTime : TscEChartBase
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private double Total { get; set; } = 0;

    private string Unit { get; set; } = "ms";

    private EChartType _options;

    private static int[] _totals = new int[] { 100, 500, 1000 };

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var initJosn = @"{""series"":[{""type"":""gauge"",""radius"":""100%"",""center"":[""25%"",""50%""],""avoidLabelOverlap"":true,""startAngle"":180,""endAngle"":0,""min"":0,""max"":500,""splitNumber"":1,""itemStyle"":{""color"":""#6946FF"",""borderColor"":""#fff"",""borderWidth"":2},""progress"":{""show"":true,""roundCap"":false,""width"":28},""pointer"":{""show"":false},""axisLine"":{""roundCap"":false,""lineStyle"":{""width"":28,""color"":[[""1"",""#05CD99""]]}},""axisTick"":{""splitNumber"":30,""distance"":24,""length"":1,""lineStyle"":{""width"":2,""color"":""#6946FF""}},""splitLine"":{""show"":false},""axisLabel"":{""show"":true,""distance"":10},""title"":{""show"":false},""detail"":{""backgroundColor"":""#fff"",""width"":""55%"",""lineHeight"":24,""height"":24,""borderRadius"":4,""offsetCenter"":[0,""0""],""valueAnimation"":true,""formatter"":""0ms"",""rich"":{""value"":{""fontSize"":""14px"",""fontWeight"":""bolder"",""color"":""#323D6F""},""unit"":{""fontSize"":""18px"",""color"":""#323D6F"",""padding"":[0,0,0,10]}}},""data"":[{""value"":0}]}]}";
        _options = new EChartType("guage", "", initJosn);
        _options.SetValue("series[0].detail.fontSize", "18px");
        _options.SetValue("series[0].detail.formatter", $"{{value|{0}}}{{unit|{"ms"}}}");
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var step = (long)Math.Floor((query.End!.Value - query.Start!.Value).TotalSeconds);
        var metric = $"round(sum by(service_name) (increase(http_server_duration_sum{{service_name=\"{query.AppId}\"}}[{MetricConstants.TIME_PERIOD}]))/sum by(service_name) (increase(http_server_duration_count{{service_name=\"{query.AppId}\"}}[{MetricConstants.TIME_PERIOD}])),1)";
        Total = 0;
        var result = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
        {
            End = query.End!.Value,
            Start = query.Start!.Value,
            Step = $"{step}s",
            Match = metric
        });
        if (result != null && result.Result != null && result.Result.Any() && result.ResultType == ResultTypes.Matrix)
        {
            var obj = ((QueryResultMatrixRangeResponse)result.Result[0]).Values!.FirstOrDefault()?[1];
            Total = obj is double.NaN || string.Equals(obj, "NaN") ? 0 : Convert.ToDouble(obj);

            if (Total - 1000 > 0)
            {
                Unit = "s";
                Total = Total / 1000.0;
            }
            else
            {
                Unit = "ms";
            }

            int currentTotal = 0;
            foreach (var item in _totals)
            {
                if (item - Total >= 0)
                {
                    currentTotal = item;
                    break;
                }
            }

            if (currentTotal == 0)
                currentTotal = (int)Math.Round(Total, 0);

            _options.SetValue("series[0].max", currentTotal);
            _options.SetValue("series[0].data[0].value", Total);
            _options.SetValue("series[0].detail.formatter", $"{{value|{Total}}}{{unit|{Unit}}}");
        }
    }
}