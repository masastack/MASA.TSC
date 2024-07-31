// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class DefaultDialog
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    [Parameter]
    public int Width { get; set; } = 400;

    private async Task UpdateVisible(bool visible)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(visible);
        }
        else
        {
            Value = false;
        }
    }
}
