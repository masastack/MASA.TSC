﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class EndpointDetail
{
    private StringNumber index = 1;
    protected override bool IsPage => true;

    private void OnSearchValueChanged(SearchData data)
    {
        Search = data;
        StateHasChanged();
    }
}
