// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<StringNumber> _trash = new List<StringNumber> { 1 };

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    public ChartPanel ChartPanel { get; set; }

    string ValueBackup { get; set; }

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

    protected override void OnInitialized()
    {
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
        if (Value.Metrics.Any() is false) Value.Metrics.Add(new());

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
        var success = !Value.Metrics.Any(x => string.IsNullOrEmpty(x.Name));
        if (!success)
        {
            await PopupService.EnqueueSnackbarAsync(T("Metrics name is required"), AlertTypes.Error);
            return;
        }
        NavigationManager.NavigateToDashboardConfigurationRecord(ConfigurationRecord.DashboardId, ConfigurationRecord.Service, ConfigurationRecord.Instance, ConfigurationRecord.Endpoint);
    }

    async Task CancelAsync()
    {
        var backUp = JsonSerializer.Deserialize<UpsertPanelDto>(ValueBackup);
        Value.Clone(backUp!);
        await NavigateToPanelConfigurationPageAsync();
    }

    void Add()
    {
        Value.Metrics.Add(new());
    }

    void Remove(PanelMetricDto metric)
    {
        Value.Metrics.Remove(metric);
    }

    async Task GetGetMetricsAsync()
    {
        await ChartPanel.ReloadAsync();
    }

    async Task MetricNameChangedAsync(PanelMetricDto metric, string metricName)
    {
        metric.Name = metricName;
        await GetGetMetricsAsync();
    }

    void ListTypeChanged(StringNumber listType)
    {
        Value.ListType = Enum.Parse<ListTypes>(listType.ToString()!);
    }

    void OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await base.InvokeAsync(base.StateHasChanged);
    }
}