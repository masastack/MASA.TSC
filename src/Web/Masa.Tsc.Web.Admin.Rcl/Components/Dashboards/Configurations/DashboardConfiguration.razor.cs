// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class DashboardConfiguration : IAsyncDisposable
{
    string _scrollElementId = Guid.NewGuid().ToString();
    string _contentElementId = Guid.NewGuid().ToString();
    IJSObjectReference? _helper;
    bool _hasNavigateTo;
    bool _serviceRelationReady;
    bool _timeRangeReady;

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    List<PanelGrids> PanelGrids { get; set; } = new();

    async Task AddPanel()
    {
        await PanelGrids.SaveUI();
        ConfigurationRecord.Panels.AdaptiveUI(new());
        if (_helper is not null)
            _ = _helper.InvokeVoidAsync("scrollBottom", _scrollElementId, _contentElementId);
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
        ConfigurationRecord.NavigateToDashboardConfiguration();
    }

    async Task SwitchEdit()
    {
        if (ConfigurationRecord.IsEdit is true)
        {
            var confirm = await OpenConfirmDialog(T("Operation confirmation"), T("Are you sure switch view mode,unsaved data will be lost"), AlertTypes.Warning);
            if (confirm)
            {
                await ConfigurationRecord.BindPanelsAsync(ApiCaller);
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
