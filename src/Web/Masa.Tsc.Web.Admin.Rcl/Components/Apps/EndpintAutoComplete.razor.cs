// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class EndpintAutoComplete
{
    [Parameter]
    public string Service
    {
        get { return _serviceName; }
        set
        {
            if (_serviceName != value)
            {
                _serviceName = value;
            }
        }
    }

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public bool Instance { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        bool isNeedLoad = parameters.TryGetValue<string>(nameof(Service), out var service) && service != _serviceName || parameters.TryGetValue<bool>(nameof(Instance), out var instance) && Instance != instance;

        await base.SetParametersAsync(parameters);
        if (isNeedLoad)
            await InvokeAsync(async () =>
            {
                await LoadDataAsync();
                await SetDefault();
                StateHasChanged();
            });
    }

    List<string> Items { get; set; } = new();

    bool _isLoading;

    string _serviceName;

    protected async Task LoadDataAsync()
    {
        _isLoading = true;
        var query = new RequestMetricListDto
        {
            Type = Instance ? MetricValueTypes.Instance : MetricValueTypes.Endpoint,
            Service = Service
        };
        var data = await ApiCaller.MetricService.GetValues(query);
        if (data != null && data.Any())
        {
            Items = data.ToList();
        }
        else if (Items.Any())
        {
            Items.Clear();
        }
        _isLoading = false;
    }

    private Task SetDefault()
    {
        if (Items.Any())
        {
            if ((string.IsNullOrEmpty(Value) || !Items.Contains(Value)))
            {
                Value = Items.FirstOrDefault()!;
                if (Value is not null)
                    return OnSelectedItemUpdate(Value);
            }
        }
        else
        {
            Value = default!;
            return OnSelectedItemUpdate(Value);
        }
        return Task.CompletedTask;
    }

    async Task OnSelectedItemUpdate(string item)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(item);
    }
}
