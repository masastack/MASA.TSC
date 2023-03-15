// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration : IAsyncDisposable
{
    string _scrollElementId = Guid.NewGuid().ToString();
    string _contentElementId = Guid.NewGuid().ToString();
    IJSObjectReference? _helper;   
    bool _hasNavigateTo;
    bool _serviceRelationReady;
    bool _timeRangeReady;

    [Inject]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    [Parameter]
    public string? ServiceName { get; set; }

    [Parameter]
    public string? InstanceName { get; set; }

    [Parameter]
    public string? EndpointName { get; set; }

    List<PanelGrids> PanelGrids { get; set; } = new();

    protected override void OnInitialized()
    {
        if (NavigationManager.Uri.Contains("record")) return;
        ConfigurationRecord.Clear();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_hasNavigateTo)
        {
            _hasNavigateTo = false;
            return;
        }
        ConfigurationRecord.Service = ServiceName;
        ConfigurationRecord.Instance = InstanceName;
        ConfigurationRecord.Endpoint = EndpointName;
        if (string.IsNullOrEmpty(DashboardId) is false && ConfigurationRecord.DashboardId != DashboardId)
        {
            ConfigurationRecord.DashboardId = DashboardId;
            await GetPanelsAsync();
        }
    }

    async Task GetPanelsAsync()
    {
        ConfigurationRecord.ClearPanels();
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(ConfigurationRecord.DashboardId));
        if (detail is not null)
        {
            ConfigurationRecord.ModelType = Enum.Parse<ModelTypes>(detail.Model);
            if (detail.Panels?.Any() is true)
            {
                ConfigurationRecord.Panels.AddRange(detail.Panels);
            }
        }

        if (ConfigurationRecord.Panels.Any() is false) ConfigurationRecord.IsEdit = true;
        Convert(ConfigurationRecord.Panels);
    }

    void Convert(List<UpsertPanelDto> panels, UpsertPanelDto? parentPanel = null)
    {
        panels.ForEach(panel => panel.ParentPanel = parentPanel);
        var chartPanels = panels.Where(panel => panel.PanelType == PanelTypes.Chart).ToList();
        var tabsPanels = panels.Where(panel => panel.PanelType == PanelTypes.Tabs).ToList();
        var tabItemPanels = panels.Where(panel => panel.PanelType == PanelTypes.TabItem).ToList();
        panels.RemoveAll(panel => panel.PanelType == PanelTypes.Chart || panel.PanelType == PanelTypes.Tabs || panel.PanelType == PanelTypes.TabItem);
        panels.AddRange(chartPanels.Select(panel => new UpsertChartPanelDto(Guid.Empty).Clone(panel)));
        panels.AddRange(tabsPanels.Select(panel => new UpsertTabsPanelDto(Guid.Empty).Clone(panel)));
        panels.AddRange(tabItemPanels.Select(panel => new UpsertTabItemPanelDto(parentPanel as UpsertTabsPanelDto).Clone(panel)));
        foreach (var panel in panels)
        {
            if (panel.ChildPanels.Any() is false) continue;
            else Convert(panel.ChildPanels, panel);
        }
    }

    async Task AddPanel()
    {
        await PanelGrids.SaveUI();
        ConfigurationRecord.Panels.AdaptiveUI(new());
        if (_helper is not null)
            _helper.InvokeVoidAsync("scrollBottom", _scrollElementId, _contentElementId);
    }

    void OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        _timeRangeReady = true;
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await base.InvokeAsync(base.StateHasChanged);
    }

    async Task SaveAsync()
    {
        await PanelGrids.SaveUI();
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(ConfigurationRecord.DashboardId), ConfigurationRecord.Panels.ToArray());
        OpenSuccessMessage(T("Save success"));
    }

    void ServiceRelationChanged((string?, string?, string?) serviceRelation)
    {
        _hasNavigateTo = true;
        _serviceRelationReady = true;
        (ConfigurationRecord.Service, ConfigurationRecord.Instance, ConfigurationRecord.Endpoint) = serviceRelation;
        NavigationManager.NavigateToDashboardConfiguration(DashboardId, ConfigurationRecord.Service, ConfigurationRecord.Instance, ConfigurationRecord.Endpoint);
    }


    async Task SwitchEdit()
    {
        if (ConfigurationRecord.IsEdit is true)
        {
            var confirm = await OpenConfirmDialog(T("Operation confirmation"), T("Are you sure switch view mode,unsaved data will be lost"), AlertTypes.Warning);
            if (confirm)
            {               
                await GetPanelsAsync();
                ConfigurationRecord.ReloadUI();
                ConfigurationRecord.IsEdit = false;
            }
            else ConfigurationRecord.IsEdit = true;
        }
        else
        {
            ConfigurationRecord.IsEdit = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _helper = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/scroll.js");
        }
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (_helper is not null)
            await _helper.DisposeAsync();
    }
}
