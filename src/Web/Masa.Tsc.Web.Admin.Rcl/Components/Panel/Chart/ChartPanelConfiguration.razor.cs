// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<string> _systemIdentities = new List<string>();
    List<StringNumber> _panelValues = new() { 1 };

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    string ValueBackup { get; set; }  

    protected override void OnInitialized()
    {
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
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

    private void Add()
    {
        Value.Metrics.Add(new());
    }

    private void Remove(PanelMetricDto metric)
    {
        Value.Metrics.Remove(metric);
    }
}