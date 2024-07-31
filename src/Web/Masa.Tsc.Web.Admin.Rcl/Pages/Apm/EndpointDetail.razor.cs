// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class EndpointDetail
{
    private StringNumber index = 1;
    protected override bool IsPage => true;

    [Parameter]
    public string Name { get; set; }

    private void OnSearchValueChanged(SearchData data)
    {
        Search = data;
        StateHasChanged();
    }
}
