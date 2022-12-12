// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch
{
    [Parameter, EditorRequired]
    public EventCallback<(string?, string?, string?, string?)> OnQueryUpdate { get; set; }

    [Parameter, EditorRequired]
    public Func<string, Task<IEnumerable<string>>> QueryServices { get; set; }

    [Parameter, EditorRequired]
    public Func<string, string, Task<IEnumerable<string>>> QueryInstances { get; set; }

    [Parameter, EditorRequired]
    public Func<string, string, Task<IEnumerable<string>>> QueryEndpoints { get; set; }

    private List<string> _services = new();
    private List<string> _instances = new();
    private List<string> _endpoints = new();

    private string _service = string.Empty;
    private string? _instance;
    private string? _endpoint;
    private string? _keyword;

    private bool _serviceSearching;
    private bool _instanceSearching;
    private bool _endpointSearching;

    private async Task SearchServices(string key)
    {
        _serviceSearching = true;
        _services = (await QueryServices.Invoke(key)).ToList();
        _serviceSearching = false;
    }

    private async Task SearchInstances(string key)
    {
        _instanceSearching = true;
        _instances = (await QueryInstances(_service, key)).ToList();
        _instanceSearching = false;
    }

    private async Task SearchEndpoints(string key)
    {
        _endpointSearching = true;
        _endpoints = (await QueryEndpoints(_service, key)).ToList();
        _endpointSearching = false;
    }

    private void KeywordChanged(string? val)
    {
        _keyword = val;
        Query();
    }

    private void Query()
    {
        NextTick(async () =>
        {
            await OnQueryUpdate.InvokeAsync((_service, _instance, _endpoint, _keyword));
            StateHasChanged();
        });
    }
}
