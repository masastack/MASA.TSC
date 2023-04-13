﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations;

public partial class DashboardConfiguration : IAsyncDisposable
{
    string _scrollElementId = Guid.NewGuid().ToString();
    IJSObjectReference? _helper;
    bool _serviceRelationReady;
    bool _timeRangeReady;
    bool _isLoading;

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    [Parameter]
    public RenderFragment HeaderContent { get; set; }

    [Parameter]
    public bool View { get; set; }

    [Parameter]
    public Func<List<UpsertPanelDto>, Task> SavePanelsAction { get; set; }

    [Parameter]
    public Func<Task<List<UpsertPanelDto>>> GetPanelsAction { get; set; }

    [Parameter]
    public string? HeaderStyle { get; set; }

    [Parameter]
    public string? HeaderClass { get; set; }

    [Parameter]
    public string? ContentStyle { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (ConfigurationRecord.Panels.Any() is false) await GetPanelsAsync();
        if (ConfigurationRecord.Panels.Any() is false) ConfigurationRecord.Panels.Add(new());
    }

    async Task GetPanelsAsync()
    {
        _isLoading = true;
        var panels = await GetPanelsAction.Invoke();
        if (panels.Any() is true)
        {
            panels.ConvertToConfigurationFormat();
            ConfigurationRecord.ClearPanels();
            ConfigurationRecord.Panels.AddRange(panels);
        }
        _isLoading = false;

        if (ConfigurationRecord.Panels.Any() is false)
        {
            ConfigurationRecord.IsEdit = true;
        }
    }

    void AddPanel()
    {
        ConfigurationRecord.Panels.AdaptiveUI(new() { AutoPosition = true });
        if (_helper is not null)
            _ = _helper.InvokeVoidAsync("scrollBottom", _scrollElementId);
    }

    async Task SaveAsync()
    {
        await SavePanelsAction.Invoke(ConfigurationRecord.Panels);
        OpenSuccessMessage(I18n.T("Save success"));
    }

    async Task SwitchEdit()
    {
        if (ConfigurationRecord.IsEdit is true)
        {
            var confirm = await OpenConfirmDialog(I18n.T("Operation confirmation"), I18n.Dashboard("Are you sure switch view mode,unsaved data will be lost"), AlertTypes.Warning);
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
        ConfigurationRecord.NavigateToConfiguration();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
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
