// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<StringNumber> _trash = new List<StringNumber> { 1 };
    ChartPanel _chartPanel;
    string _valueBackup;  

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    Dictionary<ModelTypes, ListTypes[]> TableMap = new()
    {
        [ModelTypes.All] = new[]
        {
            ListTypes.ServiceList,
            ListTypes.TopList,
        },
        [ModelTypes.Service] = new[]
        {
            ListTypes.InstanceList,
            ListTypes.EndpointList,
            ListTypes.TopList,
        },
        [ModelTypes.ServiceInstance] = new[]
        {
            ListTypes.EndpointList,
            ListTypes.TopList,
        },
        [ModelTypes.Endpoint] = new[]
        {
            ListTypes.TopList,
        },
    };

    List<string> Caculates = new List<string> { "MAX", "MIN", "SUM", "COUNT", "AVG" };

    protected override void OnInitialized()
    {
        _valueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
        if (Value.Metrics.Any() is false) Value.Metrics.Add(new());

        Value.PanelType = PanelTypes.Chart;
        InitListType();
    }

    private void InitListType()
    {
        if (TableMap[ConfigurationRecord.ModelType].Contains(Value.ListType) is false)
        {
            Value.ListType = TableMap[ConfigurationRecord.ModelType].First();
        }
    }

    async Task NavigateToPanelConfigurationPageAsync()
    {
        var success = !Value.Metrics.Any(x => string.IsNullOrEmpty(x.Expression));
        if (!success)
        {
            await PopupService.EnqueueSnackbarAsync(I18n.Dashboard("Metrics expression is required"), AlertTypes.Error);
            return;
        }
        ConfigurationRecord.IsEdit = true;
        ConfigurationRecord.NavigateToConfigurationRecord();
    }

    void Cancel()
    {
        var backUp = JsonSerializer.Deserialize<UpsertPanelDto>(_valueBackup);
        Value.Clone(backUp!);
        ConfigurationRecord.IsEdit = true;
        ConfigurationRecord.NavigateToConfigurationRecord();
    }

    void Add()
    {
        Value.Metrics.Add(new());
    }

    async Task Remove(PanelMetricDto metric)
    {
        Value.Metrics.Remove(metric);
        if (string.IsNullOrEmpty(metric.Expression) is false) await GetGetMetricsAsync();
    }

    async Task GetGetMetricsAsync()
    {
        await _chartPanel.ReloadAsync();
    }

    async Task MetricExpressionChangedAsync(PanelMetricDto metric, string metricExpression)
    {
        metric.Expression = metricExpression;
        await GetGetMetricsAsync();
    }

    void MetricNameChanged(PanelMetricDto metric, string metricName)
    {
        metric.DisplayName = metricName;
        if (string.IsNullOrEmpty(metric.Expression) is false) Value.ReloadChartData();
    }

    void MetricColorChanged(PanelMetricDto metric, string metricColor)
    {
        metric.Color = metricColor;
        if (string.IsNullOrEmpty(metric.Expression) is false) Value.ReloadChartData();
    }

    void ListTypeChanged(StringNumber listType)
    {
        Value.ListType = Enum.Parse<ListTypes>(listType.ToString()!);
    }

    void OnDateTimeUpdateAsync((DateTimeOffset?, DateTimeOffset?) times)
    {
        ConfigurationRecord.UpdateDateTimesFromTuple(times);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset?, DateTimeOffset?) times)
    {
        ConfigurationRecord.UpdateDateTimesFromTuple(times);
        await base.InvokeAsync(base.StateHasChanged);
    }

    async Task OnCaculateChange(string? value)
    {
        Value.Metrics[0].Caculate = value!;
        //_chartPanel.ReloadAsync();
    }
}