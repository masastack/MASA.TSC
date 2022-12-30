﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanel
{
    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Inject]
    ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsLoading { get; set; }

    Dictionary<string, DynamicComponentDescription> DynamicComponentMap { get; set; }

    DynamicComponentDescription CurrentDynamicComponent => DynamicComponentMap.ContainsKey(Value.ChartType) ? DynamicComponentMap[Value.ChartType] : DynamicComponentMap["e-chart"];

    protected override async Task OnInitializedAsync()
    {
        DynamicComponentMap = new()
        {
            ["table"] = new(typeof(Lists), new() { ["Value"] = Value }),
            ["e-chart"] = new(typeof(EChart), new() { ["Value"] = Value }),
        };

        await ReloadAsync();
    }

    async Task<List<QueryResultDataResponse>> GetMetricsAsync()
    {
        if(Value.ChartType is "pie" or "gauge")
        {
            return await base.ApiCaller.MetricService.GetMultiQueryAsync(new RequestMultiQueryDto()
            {
                Time = ConfigurationRecord.StartTime.UtcDateTime,
                ServiceName = ConfigurationRecord.AppName,
                Queries = Value.Metrics.Select(item => item.Name).ToList()
            });
        }
        else
        {
            return await base.ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto()
            {
                Start = ConfigurationRecord.StartTime.UtcDateTime,
                End = ConfigurationRecord.EndTime.UtcDateTime,
                ServiceName = ConfigurationRecord.AppName,
                Step = "5s",
                MetricNames = Value.Metrics.Select(item => item.Name).ToList()
            });
        }
    }

    public async Task ReloadAsync()
    {
        IsLoading = true;
        var data = await GetMetricsAsync();
        Value.SetChartData(data);
        IsLoading = false;
    }
}
