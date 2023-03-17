// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Components.Project.Charts;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class AvgResponseChart : TscEChartBase
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    public double Total { get; set; } = 0;

    public string Unit { get; set; } = "ms";

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var metric = $"round(avg by (service_name) (http_server_duration_bucket{{service_name=\"{query.AppId}\"}}>10),1)";
        Total = 0;
        var result = await ApiCaller.MetricService.GetQueryAsync(metric, query.End ?? DateTime.UtcNow);
        if (result != null && result.Result != null && result.Result.Any() && result.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Vector)
        {
            int total = (int)Math.Floor(Convert.ToDouble(((QueryResultInstantVectorResponse)result.Result[0]).Value[1]));

            if (total - 1000 > 0)
            {
                Unit = "s";
                Total = total / 1000.0;
            }
            else
            {
                Total = total;
                Unit = "ms";
            }
        }
    }
}