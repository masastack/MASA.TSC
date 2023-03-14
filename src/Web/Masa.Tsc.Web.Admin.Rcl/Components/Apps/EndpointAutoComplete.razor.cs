// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class EndpointAutoComplete
{
    bool _isLoading;
    string? _oldService;
    string? _oldInstance;

    [Parameter]
    public string? Service { get; set; }

    [Parameter]
    public string? Instance { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    public List<string> Endpoints { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        var serviceChanged = string.IsNullOrEmpty(Service) is false && _oldService != Service;
        var instanceChanged = string.IsNullOrEmpty(Instance) is false && _oldInstance != Instance;
        if (serviceChanged || instanceChanged)
        {
            _oldService = Service;
            _oldInstance = Instance;
            _isLoading = true;
            var query = new RequestMetricListDto
            {
                Type = MetricValueTypes.Endpoint,
                Service = Service,
                Instance = Instance,
            };
            var data = await ApiCaller.MetricService.GetValues(query);
            Endpoints = data ?? new();
            if (Endpoints.Any() && (string.IsNullOrEmpty(Value) || Endpoints.Contains(Value) is false))
            {
                await ValueChanged.InvokeAsync(Endpoints.First());
            }
            _isLoading = false;
        }
        if(string.IsNullOrEmpty(Service) || string.IsNullOrEmpty(Instance))
        {
            Endpoints.Clear();
        }
    }
}
