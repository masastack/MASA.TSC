﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanel
{
    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsLoading { get; set; }

    string? OldChartType { get; set; }

    string? OldConfigRecordKey { get; set; }

    //Dictionary<string, DynamicComponentDescription> DynamicComponentMap { get; set; }

    //DynamicComponentDescription CurrentDynamicComponent => DynamicComponentMap.ContainsKey(Value.ChartType) ? DynamicComponentMap[Value.ChartType] : DynamicComponentMap["e-chart"];

    protected override async Task OnInitializedAsync()
    {
        //DynamicComponentMap = new()
        //{
        //    ["table"] = new(typeof(ListPanel), new() { ["Value"] = Value, ["DataIsReady"] = !IsLoading }),
        //    ["e-chart"] = new(typeof(EChart), new() { ["Value"] = Value }),
        //};

        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (OldChartType != Value.ChartType)
        {
            if (OldChartType is "line" or "bar" or "line-area")
            {
                if (Value.ChartType is "gauge" or "heatmap" or "pie" or "table")
                {
                    await ReloadAsync();
                }
            }
            else if (OldChartType is "gauge" or "heatmap" or "pie" or "table")
            {
                if (Value.ChartType is "line" or "bar" or "line-area")
                {
                    await ReloadAsync();
                }
            }
            OldChartType = Value.ChartType;
        }

        if (OldConfigRecordKey != ConfigurationRecord.Key)
        {
            var back = OldConfigRecordKey;
            OldConfigRecordKey = ConfigurationRecord.Key;
            if (back is not null)
            {
                await ReloadAsync();
            }
        }
    }

    async Task<List<QueryResultDataResponse>> GetMetricsAsync()
    {
        if (Value.Metrics.Any(item => item.Name is not null) is false) return new();
        if (Value.ChartType is "pie" or "gauge" or "table")
        {
            return await base.ApiCaller.MetricService.GetMultiQueryAsync(new RequestMultiQueryDto()
            {
                Time = ConfigurationRecord.EndTime.UtcDateTime,
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
