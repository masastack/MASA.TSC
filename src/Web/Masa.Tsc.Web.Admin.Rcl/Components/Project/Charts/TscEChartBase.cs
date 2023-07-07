// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Project.Charts;

public partial class TscEChartBase : TscComponentBase
{
    protected bool _isLoading;
    private string lastKey = default!;
    protected string MetricEnv { get; private set; }

    [Inject]
    public IHttpContextAccessor HttpContext { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (UserContext != null && UserContext.IsAuthenticated)
        {
            var env = HttpContext.HttpContext!.User.Claims.FirstOrDefault(item => item.Type == "environment")?.Value!;
            if (string.IsNullOrEmpty(env))
            {
                MetricEnv = string.Empty;
            }
            else
            {
                MetricEnv = $"{MetricConstants.Environment}=\"{env}\"";
            }
        }
    }

    protected virtual bool CheckKeyChanged(ProjectAppSearchModel query)
    {
        var key = $"{query.AppId}_{query.Start}_{query.End}";
        if (key == lastKey)
            return false;
        lastKey = key;
        return true;
    }

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
        //await Task.Delay(200);
        await Task.CompletedTask;
    }

    protected string ToDateTimeStr(double value, string fmt)
    {
        var millionSeconds = (long)Math.Floor(value * 1000);
        return millionSeconds.ToDateTime(CurrentTimeZone).Format(fmt);
    }
}
