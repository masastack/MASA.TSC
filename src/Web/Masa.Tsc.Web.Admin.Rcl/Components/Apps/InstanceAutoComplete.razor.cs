// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class InstanceAutoComplete
{
    bool _isLoading;
    string? _oldService;

    [Parameter]
    public string? Service { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    public List<string> Instances { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        if(string.IsNullOrEmpty(Service) is false && _oldService != Service)
        {
            _oldService = Service;
            _isLoading = true;
            var query = new RequestMetricListDto
            {
                Type = MetricValueTypes.Instance,
                Service = Service
            };
            var data = await ApiCaller.MetricService.GetValues(query);
            Instances = data ?? new();
            if (Instances.Any())
            {
                if(string.IsNullOrEmpty(Value) || Instances.Contains(Value) is false)
                {
                    await ValueChanged.InvokeAsync(Instances.First());
                }
            }
            else await ValueChanged.InvokeAsync(null);
            _isLoading = false;
        }
        if (string.IsNullOrEmpty(Service))
        {
            Instances.Clear();
        }
    }
}
