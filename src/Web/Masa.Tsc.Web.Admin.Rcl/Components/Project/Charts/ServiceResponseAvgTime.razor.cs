﻿// Copyright (c) MASA Stack All rights reserved.
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
        var initJosn = @"{""series"":[{""type"":""gauge"",""radius"":""100%"",""center"":[""25%"",""50%""],""startAngle"":180,""endAngle"":0,""min"":0,""max"":500,""splitNumber"":1,""itemStyle"":{""color"":""#6946FF"",""borderColor"":""#fff"",""borderWidth"":2},""progress"":{""show"":true,""roundCap"":false,""width"":28},""pointer"":{""show"":false},""axisLine"":{""roundCap"":false,""lineStyle"":{""width"":28,""color"":[[""1"",""#05CD99""]]}},""axisTick"":{""splitNumber"":30,""distance"":24,""length"":1,""lineStyle"":{""width"":2,""color"":""#6946FF""}},""splitLine"":{""show"":false},""axisLabel"":{""show"":true,""distance"":10},""title"":{""show"":false},""detail"":{""backgroundColor"":""#fff"",""width"":""100%"",""lineHeight"":24,""height"":24,""borderRadius"":4,""offsetCenter"":[0,""0""],""valueAnimation"":true,""rich"":{""value"":{""fontSize"":""14px"",""fontWeight"":""bolder"",""color"":""#323D6F""},""unit"":{""fontSize"":""14px"",""color"":""#323D6F"",""padding"":[0,0,0,20]}}},""data"":[{""value"":100}]}]}";
        _options = new EChartType("guage", "", initJosn);
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var step = (long)Math.Floor((query.End!.Value - query.Start!.Value).TotalSeconds);
        var metric = $"round(sum by(service_name) (increase(http_server_duration_sum{{service_name=\"{query.AppId}\"}}[{step}s]))/sum by(service_name) (increase(http_server_duration_count{{service_name=\"{query.AppId}\"}}[{step}s])),1)";
        Total = 0;
        var result = await ApiCaller.MetricService.GetQueryAsync(metric, query.End!.Value);
        if (result != null && result.Result != null && result.Result.Any() && result.ResultType == ResultTypes.Vector)
        {
            var obj = ((QueryResultInstantVectorResponse)result.Result[0])!.Value![1];
            Total = obj is double.NaN || string.Equals(obj, "NaN") ? 0 : Convert.ToDouble(obj);

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
        }
    }
}