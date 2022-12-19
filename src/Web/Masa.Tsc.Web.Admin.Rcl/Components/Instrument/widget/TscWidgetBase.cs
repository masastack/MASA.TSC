// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetBase : TscComponentBase
{
    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public virtual UpsertPanelDto Value { get; set; }

    [Parameter]
    public EventCallback<UpsertPanelDto> ValueChanged { get; set; }
}
