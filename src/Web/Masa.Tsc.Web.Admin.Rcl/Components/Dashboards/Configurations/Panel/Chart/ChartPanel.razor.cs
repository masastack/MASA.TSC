// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart;

public partial class ChartPanel
{
    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

    bool IsLoading { get; set; }

    string? OldConfigRecordKey { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
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

    public async Task ReloadAsync()
    {
        IsLoading = true;
        var data = await GetMetricsAsync();
        Value.SetChartData(data, ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime);
        IsLoading = false;
    }

    async Task<List<QueryResultDataResponse>> GetMetricsAsync()
    {
        if (Value.Metrics.Any(item => item.Expression is not null) is false) return new();
        return await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto()
        {
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
            Service = ConfigurationRecord.Service,
            Instance = ConfigurationRecord.Instance,
            EndPoint = ConfigurationRecord.Endpoint,
            Step = ConfigurationRecord.StartTime.UtcDateTime.Interval(ConfigurationRecord.EndTime.UtcDateTime),
            MetricNames = Value.Metrics.Where(item => string.IsNullOrEmpty(item.Expression) is false).Select(item => item.Expression).ToList()
        });      
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