// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscEChartBase : TscComponentBase
{
    protected bool _isLoading = true;

    public async Task OnLoadAsync(ProjectAppSearchModel query)
    {
        await LoadAsync(query);
        _isLoading = false;
        StateHasChanged();
    }

    protected virtual async Task LoadAsync(ProjectAppSearchModel query)
    {
        Thread.Sleep(200);
        await Task.CompletedTask;
    }
}
