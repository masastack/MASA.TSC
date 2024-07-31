// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch
{
    [Parameter, EditorRequired]
    public EventCallback<(string? service, string? instance, string? endpoint, string? keyword)> OnQueryUpdate { get; set; }

    [Parameter, EditorRequired]
    public Func<Task<IEnumerable<string>>> QueryServices { get; set; }

    [Parameter, EditorRequired]
    public Func<string, Task<IEnumerable<string>>> QueryInstances { get; set; }

    [Parameter, EditorRequired]
    public Func<string, string?, Task<IEnumerable<string>>> QueryEndpoints { get; set; }

    [Parameter]
    public EventCallback<(DateTime, DateTime)> OnDateTimeRangeUpdate { get; set; }

    [Parameter]
    public bool PageMode { get; set; }

    [Parameter]
    public DateTime? StartDateTime { get; set; }

    [Parameter]
    public DateTime? EndDateTime { get; set; }

    [Parameter]
    public string Service { get; set; }

    [Parameter]
    public string Keyword { get; set; }

    private List<string> _services = new();
    private List<string> _instances = new();
    private List<string> _endpoints = new();

    private string? _instance;
    private string? _endpoint;

    private bool _serviceSearching;
    private bool _instanceSearching;
    private bool _endpointSearching;
    private int width = 268;
    private string _style = "flex:none;width:{0}px !important;";

    protected override async Task OnInitializedAsync()
    {
        await SearchServices();
        if (PageMode)
            width = 208;
        _style = string.Format(_style, width);
        await base.OnInitializedAsync();
    }

    public async Task SearchServices()
    {
        _serviceSearching = true;
        _services = (await QueryServices.Invoke())?.ToList()!;
        if (!string.IsNullOrEmpty(Service) && _services != null && _services.Contains(Service))
        {
            await SearchInstances();
            await SearchEndpoints();
        }
        else
        {
            Service = default!;
            _instance = default;
            _endpoint = default;
        }
        _serviceSearching = false;
    }

    private async Task SearchInstances()
    {
        _instanceSearching = true;
        _instances = (await QueryInstances(Service!))?.ToList()!;
        if (!(!string.IsNullOrEmpty(_instance) && _instances != null && _instances.Contains(_instance)))
            _instance = default!;
        _instanceSearching = false;
    }

    private async Task SearchEndpoints()
    {
        _endpointSearching = true;
        _endpoints = (await QueryEndpoints(Service!, _instance))?.ToList()!;
        if (!(!string.IsNullOrEmpty(_endpoint) && _endpoints != null && _endpoints.Contains(_endpoint)))
            _endpoint = default!;
        _endpointSearching = false;
    }

    private Task OnEnter()
    {
        return Query();
    }

    private async Task Query(bool isService = false, bool isInstance = false, bool isEndpoint = false)
    {
        if (!(isService || isInstance || isEndpoint))
        {
            await SearchServices();
        }
        else if (isService)
        {
            _instance = default;
            _endpoint = default;
            await SearchInstances();
            await SearchEndpoints();
        }
        else if (isInstance)
        {
            await SearchEndpoints();
        }

        await OnQueryUpdate.InvokeAsync((Service, _instance, _endpoint, Keyword));
        StateHasChanged();
    }

    private async Task OnDateTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        await InvokeDateTimeUpdate(range);
    }

    private async Task OnDateTimeAutoUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        await InvokeDateTimeUpdate(range);
    }

    private Task InvokeDateTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        if (range is { start: not null, end: not null })
        {
            var localStart = range.start.Value.UtcDateTime;
            var localEnd = range.end.Value.UtcDateTime;
            return OnDateTimeRangeUpdate.InvokeAsync((localStart, localEnd));
        }
        return Task.CompletedTask;
    }
}
