// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class AutoHeight
{
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    [Parameter]
    public RenderFragment? AutoHeightContent { get; set; }

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    public string AutoClass { get; set; } = "";

    [Parameter]
    public string AutoStyle { get; set; } = "";

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public bool Overflow { get; set; }
}
