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

    private double _success = 100;
    private EChartType _options = EChartConst.Pie;
    private List<QueryResultDataResponse>? _data;
    private double[] values = new double[2];
    private bool _hasData = false;

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
        _hasData = false;
        _success = 100;
        if (query == null)
            return;
        if (query.End == null)
            query.End = DateTime.UtcNow;
        if (query.Start == null)
            query.Start = query.End.Value.AddDays(-1);

        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"round(sum by(service_name) (increase(http_server_duration_count{{http_status_code!~\"5..\"}}[{MetricConstants.TIME_PERIOD}]))/sum by(service_name) (increase(http_server_duration_count[{MetricConstants.TIME_PERIOD}]))*100,0.01)"
           },
            Service = query.AppId,
            Start = query.Start.Value,
            End = query.End.Value,
            Step = query.Start.Value.Interval(query.End.Value)
        });

        if (_data != null && _data[0] != null && _data[0].ResultType == ResultTypes.Matrix && _data[0].Result != null && _data[0].Result.Any())
        {
            var first = (QueryResultMatrixRangeResponse)_data[0].Result.First();
            var values = first.Values.Select(values => Convert.ToDouble(values[1])).Where(val => !double.IsNaN(val)).ToList();
            if (values.Any())
            {
                _hasData = true;
                _success = values.Average();
                if (_success < 1 && _success > 0)
                {
                    _success = 1;
                }
                _success = Math.Floor(_success);
            }
        }

        values[0] = _success;
        values[1] = (100 - _success);

        if (_hasData && (values[0] == 0 || values[1] == 0))
        {
            _options.SetValue("series[0].itemStyle.normal.borderWidth", 0);
        }

        _options.SetValue("tooltip.formatter", "{d}%");
        _options.SetValue("legend.bottom", "1%");
        _options.SetValue("legend.itemWidth", 8);
        _options.SetValue("legend.itemHeight", 8);
        _options.SetValue("series[0].data", new object[] {GetModel(true,values[0]),
            GetModel(false,values[1]) });
    }

    private object GetModel(bool isSuccess, double value)
    {
        return new { name = isSuccess ? I18n.Team("Success") : I18n.Team("Fail"), value };
    }
}