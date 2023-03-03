// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration : IAsyncDisposable
{
    string _scrollElementId = Guid.NewGuid().ToString();
    string _contentElementId = Guid.NewGuid().ToString();
    IJSObjectReference? _helper;
    IJSObjectReference? _history;
    DotNetObjectReference<Configuration> _dotNetHelper;

    private QuickRangeKey? _defaultValue = QuickRangeKey.Last15Minutes;

    [Inject]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public string DashboardId { get; set; }

    [Parameter]
    public string ServiceName { get; set; }

    List<PanelGrids> PanelGrids { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (NavigationManager.Uri.Contains("record")) return;
        ConfigurationRecord.DashboardId = DashboardId;
        ConfigurationRecord.AppName = ServiceName;

        if (ServiceName is null)
        {
            ConfigurationRecord.Clear();
            return;
        }

        await GetPanelsAsync();
    }

    [JSInvokable]
    public void GoDashbordList()
    {
        NavigationManager.NavigateTo("/dashboard", false);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (ConfigurationRecord.DashboardId != DashboardId)
        {
            ConfigurationRecord.DashboardId = DashboardId;
            ConfigurationRecord.AppName = ServiceName;
            await GetPanelsAsync();
        }
        else if (ServiceName is not null && ServiceName != ConfigurationRecord.AppName)
        {
            ConfigurationRecord.AppName = ServiceName;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (NavigationManager.Uri.Contains("record") && string.IsNullOrEmpty(ConfigurationRecord.DashboardId))
        {
            NavigationManager.NavigateToDashboardConfiguration(DashboardId, ServiceName);
        }
    }

    async Task GetPanelsAsync()
    {
        ConfigurationRecord.Panels.Clear();
        PanelGrids.Clear();
        var detail = await ApiCaller.InstrumentService.GetDetailAsync(Guid.Parse(ConfigurationRecord.DashboardId));
        //ConfigurationRecord.Panels.Clear();
        if (detail is not null)
        {
            ConfigurationRecord.Model = detail.Model;
            ConfigurationRecord.ShowServiceCompontent = detail.Model != ModelTypes.All.ToString();
            if (ConfigurationRecord.ShowServiceCompontent is false) ConfigurationRecord.AppName = "";
        }
        if (detail?.Panels != null && detail.Panels.Any())
        {
            ConfigurationRecord.Panels.AddRange(detail.Panels);
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
        //ConfigurationRecord.Panels.Insert(0, panel);
        //await PanelGrids.First(item => item.ParentPanel is null).Gridstack!.Reload();
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

    async Task SaveAsync()
    {
        await PanelGrids.SaveUI();
        await ApiCaller.InstrumentService.UpsertPanelAsync(Guid.Parse(ConfigurationRecord.DashboardId), ConfigurationRecord.Panels.ToArray());
        OpenSuccessMessage(T("Save success"));
    }

    void ServiceNameChange(string serviceName)
    {
        ServiceName = serviceName;
        NavigationManager.NavigateToDashboardConfiguration(DashboardId, serviceName);
    }

    async Task SwitchEdit()
    {
        if (ConfigurationRecord.IsEdit is true)
        {
            var confirm = await OpenConfirmDialog(T("Operation confirmation"), T("Are you sure switch view mode,unsaved data will be lost"), AlertTypes.Warning);
            if (confirm)
            {
                ConfigurationRecord.UpdateKey();
                await GetPanelsAsync();
                Convert(ConfigurationRecord.Panels);
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
            _dotNetHelper = DotNetObjectReference.Create(this);
            _history = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/history.js");
            await _history.InvokeVoidAsync("historyToList", _dotNetHelper);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_helper is not null)
            await _helper.DisposeAsync();
        if (_history is not null)
            await _history.DisposeAsync();
        if (_dotNetHelper is not null)
            _dotNetHelper.Dispose();
    }
}
