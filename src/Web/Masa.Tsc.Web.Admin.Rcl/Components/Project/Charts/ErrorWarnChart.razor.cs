// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ErrorWarnChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private double? _success;
    private EChartType _options = EChartConst.Pie;
    private List<QueryResultDataResponse>? _data;
    private double[] values = new double[2];

    protected override void OnInitialized()
    {
        _options.SetValue("legend", new { bottom = 0, left = "center" });
        _options.SetValue("color", new string[] { "#05CD99", "#FF5252" });
        _options.SetValue("series[0]", new
        {
            type = "pie",
            radius = "76%",
            itemStyle = new
            {
                normal = new
                {
                    borderColor = "#FFFFFF",
                    borderWidth = 2
                }
            },
            label = new { show = false },
            center = new string[] { "50%", "43%" }
        });
        base.OnInitialized();
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        _success = null;
        if (query == null)
            return;
        if (query.End == null)
            query.End = DateTime.UtcNow;
        if (query.Start == null)
            query.Start = query.End.Value.AddDays(-1);

        var step = (long)Math.Floor((query.End.Value - query.Start.Value).TotalSeconds);
        _data = await ApiCaller.MetricService.GetMultiQueryAsync(new RequestMultiQueryDto
        {
            Queries = new List<string> {
            $"round(sum by(service_name) (increase(http_server_duration_count{{http_status_code!~\"5..\"}}[{step}s])),1)",
            $"round(sum by(service_name) (increase(http_server_duration_count[{step}s])),1)"
           },
            Service = query.AppId,
            Time = query.End.Value,
        });

        var index = 0;
        foreach (var item in _data)
        {
            if (item != null && item.ResultType == ResultTypes.Vector && item.Result != null && item.Result.Any())
            {
                var first = (QueryResultInstantVectorResponse)item.Result.First();
                values[index++] = Convert.ToDouble(first.Value![1]);
            }
        }

        if (values[1] == 0)
        {
            values[0] = 1;
            _success = 100;
        }
        else
        {
            _success = Math.Round(values[0] * 100 / values[1], 2, MidpointRounding.ToNegativeInfinity);
        }

        if (_success.HasValue)
        {
            _success = Math.Round(_success.Value, 2, MidpointRounding.ToNegativeInfinity);
        }

        if (_data?.Any(item => item?.Result?.Any() is true) is true || Math.Round(values[0], 2) <= 0 || Math.Round(values[1]) <= 0)
        {
            _options.SetValue("series[0].itemStyle.normal.borderWidth", 0);
        }

        _options.SetValue("tooltip.formatter", "{d}%");
        _options.SetValue("legend.bottom", "1%");
        _options.SetValue("series[0].data", new object[] {GetModel(true,values[0]),
            GetModel(false,values[1]-values[0]) });
    }

    private static object GetModel(bool isSuccess, double value)
    {
        return new { name = isSuccess ? "Success" : "Fail", value = Math.Round(value, 2) };
    }
}