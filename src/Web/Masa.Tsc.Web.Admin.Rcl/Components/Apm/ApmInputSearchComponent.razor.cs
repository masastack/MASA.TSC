// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

public partial class ApmInputSearchComponent
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    private async Task OnValueChange()
    {
        await ValueChanged.InvokeAsync(Value);
    }
}