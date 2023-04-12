// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ServiceCallChart
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    private EChartType _options = EChartConst.Line;

    private List<QueryResultDataResponse>? _data;
    private DateTime StartTime = DateTime.UtcNow.AddDays(-1);
    private DateTime EndTime = DateTime.UtcNow;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _options.SetValue("series[0].smooth", true);
        _options.SetValue("series[0].showSymbol", false);
        _options.SetValue("yAxis.splitLine", new
        {
            lineStyle = new
            {
                color = "#E4E8F3",
                type = "dashed",
                width = 1
            }
        });
        _options.SetValue("xAxis.axisLine", new
        {
            lineStyle = new
            {
                color = "#FF5252",
                type = "solid",
                width = 1
            }
        });
        _options.SetValue("xAxis.axisLabel", new
        {
            textStyle = new
            {
                color = "#A3AED0"
            }
        });
        _options.SetValue("tooltip", new
        {
            trigger = "axis",
            axisPointer = new
            {
                type = "cross"
            }
        });
        _options.SetValue("series[0].lineStyle", new
        {
            type = "solid",
            width = 3,
            color = "#FF5252"
        });
        _options.SetValue("series[0].areaStyle", new { color = new { colorStops = new[] { new { offset = 0, color = "rgba(255, 82, 82, 1)" }, new { offset = 1, color = "rgba(255, 82, 82, 0)" } }, x = 0, y = 0, x2 = 0, y2 = 1, type = "linear", global = false } });

        var formatter = @$"
               <div style='width:136px;margin:-2px'>
  		        <div style='display:flex;justify-content: space-between;padding-bottom:8px;'>
  		          <div style='display:flex;align-items:center;'>
    		          <div style='width:8px;height:8px;background-color:red;border0radius:1px'></div>
    		          <div>&nbsp;{I18n.Team("Calls")}</div>
  		          </div>
  		          <div>{{c}}</div>
  		        </div>
  		        <div style='border-top:1px solid #E4E4E6;margin:0 -8px;'></div>
  		        <div style='text-align:right;padding-top:2px;'>{{b}}</div>
		       </div>";
        _options.SetValue("tooltip.formatter", formatter);
        _options.SetValue("series[0].color", "#FF5252");
        _options.SetValue("series[0].name", I18n.Team("Service Load"));
        _options.SetValue("legend.data[0]", new { name = I18n.Team("Service Load"), icon = "square" });
        _options.SetValue("legend.top", "bottom");
        _options.SetValue("grid.bottom", 60);
    }

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        if (query == null)
            return;
        if (query.Start.HasValue)
            StartTime = query.Start.Value;
        if (query.End.HasValue)
            EndTime = query.End.Value;

        var step = StartTime.Interval(EndTime);
        _data = await ApiCaller.MetricService.GetMultiRangeAsync(new RequestMultiQueryRangeDto
        {
            MetricNames = new List<string> {
                $"round(sum by(service_name)(increase(http_server_duration_count[1m])),0.01)"
            },
            Service = query.AppId,
            Start = StartTime,
            End = EndTime,
            Step = step
        });

        SetData();
    }

    private void SetData()
    {
        List<string> values = new();
        var timeSpans = new List<double>();

        if (_data != null && _data[0] != null && _data[0].ResultType == ResultTypes.Matrix && _data[0].Result != null && _data[0].Result.Any())
        {
            timeSpans.AddRange(((QueryResultMatrixRangeResponse)_data[0].Result![0]).Values!.Select(values => Convert.ToDouble(values[0])));
            values = ((QueryResultMatrixRangeResponse)_data[0].Result![0])!.Values!.Select(values => values[1].ToString()!).ToList();
        }
        var format = StartTime.Format(EndTime);
        _options.SetValue("xAxis.data", timeSpans.Select(value => ToDateTimeStr(value, format)));
        _options.SetValue("series[0].data", values);
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        SetData();
        StateHasChanged();
    }
}
