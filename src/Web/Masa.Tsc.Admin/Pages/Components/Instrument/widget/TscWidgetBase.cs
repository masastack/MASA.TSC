// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscWidgetBase : TscComponentBase
{
    [Parameter]
    public int Index { get; set; }

    [Parameter]
    public StringNumber Height { get; set; }

    [Parameter]
    public StringNumber Width { get; set; }

    [Parameter]
    public string Style { get; set; }

    [Parameter]
    public string Class { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }
}
