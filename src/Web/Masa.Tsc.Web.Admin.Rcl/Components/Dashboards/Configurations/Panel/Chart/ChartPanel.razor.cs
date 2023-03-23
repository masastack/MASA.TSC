// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart;

public partial class ChartPanel
{
    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsLoading { get; set; }

    ChartTypes? OldChartType { get; set; }

    string? OldConfigRecordKey { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (OldChartType != Value.ChartType)
        {
            if (OldChartType is ChartTypes.Line or ChartTypes.Bar or ChartTypes.LineArea)
            {
                if (Value.ChartType is ChartTypes.Gauge or ChartTypes.Heatmap or ChartTypes.Pie or ChartTypes.Table)
                {
                    await ReloadAsync();
                }
            }
            else if (OldChartType is ChartTypes.Gauge or ChartTypes.Heatmap or ChartTypes.Pie or ChartTypes.Table)
            {
                if (Value.ChartType is ChartTypes.Line or ChartTypes.Bar or ChartTypes.LineArea)
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
        if (Value.Metrics.Any(item => item.Expression is not null) is false) return new();
        if (Value.ChartType is ChartTypes.Pie or ChartTypes.Gauge or ChartTypes.Table)
        {
            return await ApiCaller.MetricService.GetMultiQueryAsync(new RequestMultiQueryDto()
            {
                Time = ConfigurationRecord.EndTime.UtcDateTime,
                Service = ConfigurationRecord.Service,
                Instance = ConfigurationRecord.Instance,
                EndPoint = ConfigurationRecord.ConvertEndpoint,
                Queries = Value.Metrics.Select(item => item.Expression).ToList()
            });
        }
        else
        {
            return await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto()
            {
                Start = ConfigurationRecord.StartTime.UtcDateTime,
                End = ConfigurationRecord.EndTime.UtcDateTime,
                Service = ConfigurationRecord.Service,
                Instance = ConfigurationRecord.Instance,
                EndPoint = ConfigurationRecord.ConvertEndpoint,
                Step = ConfigurationRecord.StartTime.UtcDateTime.Interval(ConfigurationRecord.EndTime.UtcDateTime),
                MetricNames = Value.Metrics.Select(item => item.Expression).ToList()
            });
        }
    }

    public async Task ReloadAsync()
    {
        IsLoading = true;
        var data = await GetMetricsAsync();
        Value.SetChartData(data, ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime);
        IsLoading = false;
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        if (Value.ChartType is ChartTypes.Line or ChartTypes.Bar or ChartTypes.LineArea)
        {
            Value.SetTimeZoneChange();
            StateHasChanged();
        }
    }
}