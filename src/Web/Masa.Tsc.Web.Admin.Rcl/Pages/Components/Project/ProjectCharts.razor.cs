// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class ProjectCharts
{
    private string _borderStyle = "border";

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }
        return base.OnAfterRenderAsync(firstRender);
    }

    public async Task OnSearchAsync()
    {
        await Task.CompletedTask;
    }
}