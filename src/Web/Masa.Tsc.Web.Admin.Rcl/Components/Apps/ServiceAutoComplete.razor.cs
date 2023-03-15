// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class ServiceAutoComplete
{
    bool _firstValueChanged;
    bool _isLoading;

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    [Parameter]
    public bool Metric { get; set; }

    [Parameter]
    public List<AppDetailModel> Services { get; set; }

    public AppDetailModel? CurrentApp => Services?.FirstOrDefault(app => app.Identity == Value);

    protected override async Task OnInitializedAsync()
    {
        if (Metric)
        {
            _isLoading = true;
            var data = await ApiCaller.MetricService.GetValues(new RequestMetricListDto { Type = MetricValueTypes.Service });
            if (data != null && data.Any())
            {
                Services = data.Select(item => new AppDetailModel
                {
                    Identity = item,
                    Name = item
                }).ToList();
            }
            _isLoading = false;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_firstValueChanged is false && string.IsNullOrEmpty(Value) && Services?.Any() is true)
        {
            _firstValueChanged = true;
            var value = Services.First().Identity;
            await ValueChanged.InvokeAsync(value);
        }
    }
}
