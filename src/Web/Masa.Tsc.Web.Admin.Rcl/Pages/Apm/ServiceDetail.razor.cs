// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class ServiceDetail
{
    protected override bool IsPage => true;
    private StringNumber index = 1;

    [Parameter]
    public string Name { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Search.Endpoint = default!;
    }
}
