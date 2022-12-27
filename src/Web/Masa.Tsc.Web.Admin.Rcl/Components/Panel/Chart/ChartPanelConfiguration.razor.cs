// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    List<StringNumber> _panelValues = new() { 1 };

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    string ValueBackup { get; set; }

    Dictionary<string, DynamicComponentDescription> DynamicComponentMap { get; set; }

    DynamicComponentDescription CurrentDynamicComponent => DynamicComponentMap.ContainsKey(Value.ChartType) ? DynamicComponentMap[Value.ChartType] : DynamicComponentMap["e-chart"];

    protected override void OnInitialized()
    {
        DynamicComponentMap = new()
        {
            ["table"] = new(typeof(TableConfiguration), new() { ["Value"] = Value }),
            ["top-list"] = new(typeof(TopListConfiguration), new() { ["Value"] = Value }),
            ["e-chart"] = new(typeof(EChartConfiguration), new() { ["Value"] = Value }),
        };
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
    }

    void NavigateToPanelConfigurationPage()
    {
        NavigationManager.NavigateTo($"/dashboard/configuration/{DashboardId}/record");
    }

    void Cancel()
    {
        var backUp = JsonSerializer.Deserialize<UpsertPanelDto>(ValueBackup);
        Value.Clone(backUp!);
        NavigateToPanelConfigurationPage();
    }
}