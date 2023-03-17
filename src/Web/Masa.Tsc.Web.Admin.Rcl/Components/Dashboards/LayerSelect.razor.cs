// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards;

public partial class LayerSelect
{
    bool _firstValueChanged;

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<string>>? ValueExpression { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    protected List<string> Layers { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.MetricService.GetValues(new RequestMetricListDto { Type = MetricValueTypes.Layer });
        Layers = data ?? new();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_firstValueChanged is false && string.IsNullOrEmpty(Value) && Layers.Any())
        {
            _firstValueChanged = true;
            var value = Layers.First();
            await ValueChanged.InvokeAsync(value);
        }
    }
}
