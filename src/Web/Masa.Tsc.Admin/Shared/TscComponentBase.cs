// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Blazor;

namespace Masa.Tsc.Admin.Rcl.Shared;

public class TscComponentBase: ComponentBase
{
    [Inject]
    public ILogger<TscComponentBase> Logger { get; set; }

    [Inject]
    public IPopupService PopupService { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        Logger.LogInformation("OnAfterRender");

        base.OnAfterRender(firstRender);
    }
}
