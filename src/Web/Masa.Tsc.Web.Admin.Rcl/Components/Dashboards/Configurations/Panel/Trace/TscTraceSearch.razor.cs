// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch
{
    [Parameter, EditorRequired]
    public EventCallback<(string?, string?, string?, string?)> OnQueryUpdate { get; set; }

    [Parameter, EditorRequired]
    public Func<Task<IEnumerable<string>>> QueryServices { get; set; }

    [Parameter, EditorRequired]
    public Func<string, Task<IEnumerable<string>>> QueryInstances { get; set; }

    [Parameter, EditorRequired]
    public Func<string, string?, Task<IEnumerable<string>>> QueryEndpoints { get; set; }

    [Parameter]
    public string? TraceId { get; set; }

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

    protected override async Task OnInitializedAsync()
    {
        _keyword = TraceId;
        await SearchServices();
        await base.OnInitializedAsync();
    }

    private async Task SearchServices()
    {
        _serviceSearching = true;
        _services = (await QueryServices.Invoke()).ToList();
        _serviceSearching = false;
    }

    private async Task SearchInstances()
    {
        _instanceSearching = true;
        _instances = (await QueryInstances(_service)).ToList();
        _instanceSearching = false;
    }

    private async Task SearchEndpoints()
    {
        _endpointSearching = true;
        _endpoints = (await QueryEndpoints(_service, _instance)).ToList();
        _endpointSearching = false;
    }

    private void KeywordChanged(string? val)
    {
        _keyword = val;
        Query();
    }

    private void Query(bool isService = false, bool isInstance = false)
    {
        NextTick(async () =>
        {
            if (isService)
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

            await OnQueryUpdate.InvokeAsync((_service, _instance, _endpoint, _keyword));
            StateHasChanged();
        });
    }
}
