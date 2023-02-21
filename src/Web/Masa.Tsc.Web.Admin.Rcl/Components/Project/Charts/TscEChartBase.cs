// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscEChartBase : TscComponentBase
{
    protected bool _isLoading;

    internal async Task OnLoadAsync(ProjectAppSearchModel query)
    {
        _isLoading = true;
        StateHasChanged();
        await LoadAsync(query);
        _isLoading = false;
        StateHasChanged();
    }

    internal virtual async Task LoadAsync(ProjectAppSearchModel query)
    {
        await Task.Delay(200);
        await Task.CompletedTask;
    }

    protected string ToDateTimeStr(double value,string fmt)
    {
        var millionSeconds = (long)Math.Floor(value * 1000);
        return millionSeconds.ToDateTime(CurrentTimeZone).Format(CurrentTimeZone, fmt);
    }
}
