﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<StringNumber> _panelValues = new() { 1 };
    string _listType = string.Empty;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    [Parameter]
    public string? ServiceName { get; set; }

    [Parameter]
    public string? Model { get; set; }

    public ChartPanel ChartPanel { get; set; }

    bool IsLoading { get; set; }

    string ValueBackup { get; set; }

    protected override void OnInitialized()
    {
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
        if (Value.Metrics.Any() is false) Value.Metrics.Add(new());
    }

    async Task NavigateToPanelConfigurationPageAsync()
    {
        var success = !Value.Metrics.Any(x => string.IsNullOrEmpty(x.Name));
        if (!success)
        {
            await PopupService.AlertAsync(T("Metrics name is required"), AlertTypes.Error);
            return;
        }
        NavigationManager.NavigateToDashboardConfigurationRecord(DashboardId, ServiceName);
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
        IsLoading = true;
        await ChartPanel.ReloadAsync();
        IsLoading = false;
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
}