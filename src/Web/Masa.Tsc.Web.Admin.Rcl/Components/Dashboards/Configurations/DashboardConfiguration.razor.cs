// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class DashboardConfiguration : IAsyncDisposable
{
    string _scrollElementId = Guid.NewGuid().ToString();
    string _contentElementId = Guid.NewGuid().ToString();
    IJSObjectReference? _helper;
    bool _serviceRelationReady;
    bool _timeRangeReady;

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public RenderFragment Header { get; set; }

    [Parameter]
    public Func<List<UpsertPanelDto>, Task> SavePanelsAction { get; set; }

    [Parameter]
    public Func<Task<List<UpsertPanelDto>>> GetPanelsAction { get; set; }

    List<PanelGrids> PanelGrids { get; set; } = new();

    async Task AddPanel()
    {
        await PanelGrids.SaveUI();
        ConfigurationRecord.Panels.AdaptiveUI(new());
        if (_helper is not null)
            _ = _helper.InvokeVoidAsync("scrollBottom", _scrollElementId, _contentElementId);
    }

    async Task GetPanelsAsync()
    {
        var panels = await GetPanelsAction.Invoke();
        if (panels.Any() is true)
        {
            Convert(panels);
            ConfigurationRecord.ClearPanels();
            ConfigurationRecord.Panels.AddRange(panels);
        }

        if (ConfigurationRecord.Panels.Any() is false) ConfigurationRecord.IsEdit = true;

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
    }

    async Task SaveAsync()
    {
        await PanelGrids.SaveUI();
        await SavePanelsAction.Invoke(ConfigurationRecord.Panels);
        OpenSuccessMessage(T("Save success"));
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

    void ServiceRelationChanged((string?, string?, string?) serviceRelation)
    {
        _serviceRelationReady = true;
        (ConfigurationRecord.Service, ConfigurationRecord.Instance, ConfigurationRecord.Endpoint) = serviceRelation;
        ConfigurationRecord.NavigateToDashboardConfiguration();
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
