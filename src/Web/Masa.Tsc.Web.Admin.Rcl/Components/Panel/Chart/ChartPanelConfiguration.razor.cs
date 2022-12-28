// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<string> _systemIdentities = new List<string>();
    List<StringNumber> _panelValues = new() { 1 };
    string _listType = string.Empty;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Parameter]
    public Func<Task<List<QueryResultDataResponse>>> GetMetrics { get; set; }

    bool IsLoading { get; set; }

    string ValueBackup { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
        await GetGetMetricsAsync();
    }

    public void ListTypeChanged(string type)
    {
        _listType = type;
        //this.StateHasChanged();
    }

    void NavigateToPanelConfigurationPage()
    {
        NavigationManager.NavigateTo($"/dashboard/configuration/record");
    }

    void Cancel()
    {
        var backUp = JsonSerializer.Deserialize<UpsertPanelDto>(ValueBackup);
        Value.Clone(backUp!);
        NavigateToPanelConfigurationPage();
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
        Value.SetChartData(await GetMetrics());
        IsLoading = false;
    }

    async Task MetricNameChangedAsync(PanelMetricDto metric, string metricName)
    {
        metric.Name = metricName;
        await GetGetMetricsAsync();
    }
}