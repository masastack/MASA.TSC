// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ApdexChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    public float Total { get; set; }

    private EChartType _options = EChartConst.Line;

    private List<QueryResultDataResponse>? _data;

    private DateTime StartTime;

    private DateTime EndTime;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _options.SetValue("yAxis", new { type = "value", show = false });
        _options.SetValue("xAxis.show", false);
        _options.SetValue("tooltip", new
        {
            trigger = "axis",
            axisPointer = new
            {
                type = "cross"
            }
        });
        _options.SetValue("grid", new[] { new {
            top=10,
            left=10,
            right=10,
            bottom=10
        } });
        _options.SetValue("series[0].smooth", true);
        _options.SetValue("series[0].showSymbol", false);
        _options.SetValue("series[0].itemStyle", new
        {
            normal = new
            {
                lineStyle = new
                {
                    type = "solid",
                    width = 6,
                    color = new
                    {
                        colorStops = new[] {
                            new {
                            offset=0,
                            color="rgba(255, 82, 82, 1)",
                            },
                            new {
                            offset=1,
                            color="rgba(255, 82, 82, 0)",
                            }
                        },
                        x = 0,
                        y = 0,
                        x2 = 0,
                        y2 = 1,
                        type = "linear",
                        global = false
                    }
                }
            }
        });
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        var step = query.Start!.Value.Interval(query.End!.Value);
        string metric = $@"round((sum(rate(http_server_duration_bucket{{le=""250""}}[{step}])) by (service_name) + 
                                 sum(rate(http_server_duration_bucket{{le=""1000""}}[{step}])) by (service_name)
                               ) /2/sum(rate(http_server_duration_bucket{{le=""+Inf""}}[{step}])) by (service_name),0.0001)";
        StartTime = query.Start.Value;
        EndTime = query.End.Value;
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> { metric },
            Start = query.Start.Value,
            End = query.End.Value,
            Service = query.AppId,
            Step = step
        });
        SetData();
    }

    private void SetData()
    {
        if (_data != null && _data[0] != null && _data[0].ResultType == ResultTypes.Matrix && _data[0].Result != null && _data[0].Result!.Any())
        {
            var seriesData = ((QueryResultMatrixRangeResponse)_data[0].Result!.First()).Values!.Select(items => (string)items[1]).ToArray();
            var timeSpans = ((QueryResultMatrixRangeResponse)_data[0].Result!.First()).Values!.Select(items => Convert.ToDouble(items[0])).ToArray();
            seriesData = Caculate(seriesData);
            Total = toFloat(seriesData.Last());
            var format = StartTime.Format(EndTime);
            _options.SetValue("xAxis.data", timeSpans.Skip(1).Select(value => ToDateTimeStr(value, format)));
            _options.SetValue("series[0].data", seriesData);
        }
        else
        {
            _options.SetValue("xAxis.data", Array.Empty<string>());
            _options.SetValue("series[0].data", Array.Empty<string>());
            Total = 0;
        }
    }

    private float toFloat(string strValue)
    {
	    if (!float.TryParse(strValue, out float value))
	    {
	       value = 0;
	    }
        return value;
    }

    private string[] Caculate(string[] data)
    {
        if (data == null || !data.Any())
            return data!;

        var values = data.Select(str => double.Parse(str)).ToArray();

        var result = data[1..];
        var index = 1;
        do
        {
            double pre = values[index - 1], current = values[index];
            if (pre is double.NaN || current is double.NaN)
                result[index - 1] = double.NaN.ToString();
            else
                result[index - 1] = DoubleToString(Math.Round((current - pre) * 100.0 / pre, 2));
            index++;
        }
        while (data.Length - index > 0);
        return result;
    }

    private string DoubleToString(double value)
    {
        if (value > 0)
            return value.ToString("+0.##");
        else if (value < 0)
            return value.ToString("-0.##");
        else
            return value.ToString();
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        SetData();
        StateHasChanged();
    }
}