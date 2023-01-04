// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ObserveChart : TscEChartBase
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; } = "服务平均响应(毫秒/秒)";

    [Parameter]
    public string SubText { get; set; } = "该时间段内服务平均响应时间 平均值";

    //private EChartType _options = EChartConst.Pie;

    public double Total { get; set; } = 0;

    public string Unit { get; set; } = "ms";

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var metric = $"avg by (service_name) (http_server_duration_bucket{{service_name=\"{query.AppId}\"}}>10)";
        Total = 0;
        var result= await ApiCaller.MetricService.GetQueryAsync(metric, query.End ?? DateTime.Now);
        if (result != null && result.Result != null && result.Result.Any() && result.ResultType == Utils.Data.Prometheus.Enums.ResultTypes.Vector)
        {
           int total=(int)Math.Floor(Convert.ToDouble(((QueryResultInstantVectorResponse)result.Result[0]).Value[1]));

            if (total - 1000 > 0)
            {
                Unit = "s";
                Total = total / 1000.0 ;
            }
            else
            {
                Total = total;
                Unit = "ms";
            }
        }

        //_options.SetValue("legend", new { bottom = 10, left = "center" });
        //_options.SetValue("title", new
        //{
        //    text = "69%",
        //    x = "center",
        //    y = "center",
        //    textStyle = new
        //    {
        //        fontSize = 30
        //    }
        //});
        //_options.SetValue("series[0].radius", new List<string> { "40%", "70%" });
        //_options.SetValue("series[0].label", new { show = false });
        //await Task.CompletedTask;
    }
}