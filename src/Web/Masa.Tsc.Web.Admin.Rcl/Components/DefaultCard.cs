// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class DefaultCard : MCard
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (Class.Contains("pa-6") is false)
            Class += " pa-6";
    }
}
