// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCharts
{
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {           
            //StateHasChanged();
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    public async Task OnSearchAsync()
    {
        await Task.CompletedTask;
    }

    private ProjectAppSearchModel _query;

    public async Task OnLoadDataAsyc(ProjectAppSearchModel query)
    {
        _query= query;
        await Task.CompletedTask;
        StateHasChanged();
        //await Task.Delay(500);
        //StateHasChanged();
    }
}