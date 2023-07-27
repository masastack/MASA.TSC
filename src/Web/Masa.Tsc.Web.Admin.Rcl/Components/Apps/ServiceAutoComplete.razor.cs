// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class ServiceAutoComplete
{
    bool _firstValueChanged;
    bool _isLoading;
    Guid _key = Guid.Empty;

    [Inject]
    public IPmClient PmClient { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    [Parameter]
    public EventCallback DataReady { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    [Parameter]
    public bool Metric { get; set; } = true;

    [Parameter]
    public List<AppDetailModel> Services { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public string? Label { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Services is null)
        {
            _isLoading = true;
            if (Metric)
            {
                var data = CombineServices(await ApiCaller.MetricService.GetValues(new RequestMetricListDto { Type = MetricValueTypes.Service }), await PmClient.AppService.GetListAsync());
                if (data != null && data.Any())
                {
                    Services = data;
                }
            }
            else
            {
                var data = await PmClient.AppService.GetListAsync();
                if (data != null && data.Any())
                    Services = data;
            }
            _isLoading = false;
            if (DataReady.HasDelegate) await DataReady.InvokeAsync();
        }
    }

    private List<AppDetailModel> CombineServices(List<string> metricServices, List<AppDetailModel> pmServices)
    {
        var result = new List<AppDetailModel>();
        if (metricServices == null || !metricServices.Any())
        {
            if (pmServices != null && pmServices.Any())
                return pmServices.Select(pm => new AppDetailModel { Identity = pm.Identity, Name = pm.Name }).ToList();
            else if (!string.IsNullOrEmpty(Value))
                return new List<AppDetailModel> { new AppDetailModel { Name = Value, Identity = Value } };

            return result;
        }

        foreach (var name in metricServices)
        {
            var pmService = pmServices?.FirstOrDefault(p => p.Identity == name);
            if (pmService != null)
                result.Add(pmService);
            else
                result.Add(new AppDetailModel
                {
                    Identity = name,
                    Name = name
                });
        }
        return result;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_firstValueChanged is false && string.IsNullOrEmpty(Value) && Services?.Any() is true)
        {
            _firstValueChanged = true;
            var value = Services[0].Identity;
            await ValueChanged.InvokeAsync(value);
        }
    }

    public async Task OnBlurAsync(FocusEventArgs focusEventArgs)
    {
        if (Value.IsNullOrEmpty() && Services != null && Services.Any())
        {
            _key = Guid.NewGuid();
            var value = Services[0].Identity;
            await ValueChanged.InvokeAsync(value);
        }
    }

    public async Task InputValueChangedAsync(string? newValue)
    {
        if (!newValue.IsNullOrEmpty())
        {
            await ValueChanged.InvokeAsync(newValue);
        }
        else
        {
            Value = null;
        }
    }

    public AppDetailModel? CurrentApp(string? service = null) => Services?.FirstOrDefault(app => app.Identity == (service ?? Value));
}
