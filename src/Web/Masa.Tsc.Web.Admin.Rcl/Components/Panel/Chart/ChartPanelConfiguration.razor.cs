// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class ChartPanelConfiguration : TscComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public UpsertChartPanelDto Value { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    string ValueBackup { get; set; }

    Dictionary<string, DynamicComponentDescription> DynamicComponentMap { get; set; }
    List<StringNumber> _panelValues = new() { 1 };
    string _listType = string.Empty;

    DynamicComponentDescription CurrentDynamicComponent => DynamicComponentMap.ContainsKey(Value.ChartType) ? DynamicComponentMap[Value.ChartType] : DynamicComponentMap["e-chart"];

    protected override void OnInitialized()
    {
        DynamicComponentMap = new()
        {
            ["table"] = new(typeof(Lists), new()
            {
                ["Value"] = Value,
                ["OnListTypeChanged"] = EventCallback.Factory.Create<string>(this, ListTypeChanged)
            }),
            ["e-chart"] = new(typeof(EChartConfiguration), new() { ["Value"] = Value }),
        };
        ValueBackup = JsonSerializer.Serialize<UpsertPanelDto>(Value);
    }

    public void ListTypeChanged(string type)
    {
        _listType = type;
        //this.StateHasChanged();
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