﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class DefaultCard : MCard
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (Class.Contains("pa-") is false)
            Class += " pa-6";
    }
}
