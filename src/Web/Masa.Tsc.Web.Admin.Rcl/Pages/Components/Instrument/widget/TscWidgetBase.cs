// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Shared;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

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
