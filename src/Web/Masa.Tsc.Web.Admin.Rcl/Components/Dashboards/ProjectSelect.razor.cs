// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards;

public partial class ProjectSelect
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<string>>? ValueExpression { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    protected List<string> Projects { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.MetricService.GetValues(new RequestMetricListDto { Type = MetricValueTypes.Service });
        if (data != null)
        {
            Projects = data;
        }
        await base.OnInitializedAsync();
    }
}
