// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetChart : TscWidgetBase
{
    private EChartPanelDto _panelValue = new();

    private List<EChartPanelMetricItemModel> _metrics = new();

    private object _options = new();

    private EChartType _type;

    public override PanelDto Value
    {
        get => _panelValue;
        set
        {
            if (value is null) _panelValue = new()
            {
                Title = "EChart",
                Description = string.Empty,
                ChartType = "line"
            };
            else if (value is EChartPanelDto dto) _panelValue = dto;
            else
            {
                _panelValue.Id = value.Id;
                _panelValue.ParentId = value.ParentId;
                _panelValue.InstrumentId = value.InstrumentId;
            }
            if (_panelValue.Metrics != null && _panelValue.Metrics.Any())
            {
                _metrics = _panelValue.Metrics.Select(x => new EChartPanelMetricItemModel
                {
                    Caculate = x.Caculate,
                    Color = x.Color,
                    Name = x.Name,
                    Unit = x.Unit
                }).ToList();
            }
            else
            {
                if (_panelValue.Metrics == null)
                    _metrics = new List<EChartPanelMetricItemModel>();
                else
                    _metrics.Clear();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (_panelValue.Metrics == null)
            _panelValue.Metrics = new();

        //_panelValue.Metrics.Add(new PanelMetricDto
        //{
        //    Caculate = "sum by(job) (rate (process_resident_memory_bytes[30s]))/(2^20)",
        //    DisplayName = "process_resident_memory_bytes",
        //    Unit = ""

        //});
        //_panelValue.Metrics.Add(new PanelMetricDto
        //{
        //    Caculate = "sum(up)",
        //    DisplayName = "s_upname",
        //    Unit = ""
        //});
        await SetChartType(_panelValue.ChartType, LoadDataAsync);
        await base.OnInitializedAsync();
    }

    private async Task<QueryResultDataResponse[]> LoadDataAsync()
    {
        if (_metrics != null && _metrics.Any())
        {
            var requestData = new List<QueryResultDataResponse>();
            foreach (var metric in _metrics)
            {
                var data = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
                {
                    Match = metric.Caculate,
                    Start = DateTime.Now.Date.AddDays(-10),
                    End = DateTime.Now,
                    Step = "5m"
                });
                requestData.Add(data);
            }
            return requestData.ToArray();
        }
        return default!;
    }

    private IEnumerable<ValueTuple<string, IEnumerable<double>, IEnumerable<string>>> asdasdasd(QueryResultDataResponse data, PanelMetricDto metricSetting)
    {
        List<ValueTuple<string, IEnumerable<double>, IEnumerable<string>>> result = new();
        switch (data.ResultType)
        {
            case ResultTypes.Matrix:
                {
                    var matrixDatas = data.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
                    foreach (var item in matrixDatas)
                    {
                        result.Add(ValueTuple.Create(GetLabelName(item.Metric,default!), item.Values.Select(it => (double)it[0]).ToArray(), item.Values.Select(it => (string)it[1]).ToArray()));
                    }
                }
                break;
            case ResultTypes.Vector:
                {
                    //var instantData = data.Result.Select(item => (QueryResultInstantVectorResponse)item).ToArray();
                    //foreach (var item in instantData)
                    //{
                    //    result.Add(ValueTuple.Create( String.Empty, instantData.Select(item => (double)item.Value[0]).ToArray(), instantData.Select(item => (string)item.Value[1]).ToArray()));
                    //}                    
                }
                break;
            default:
                {
                    //result.Add(ValueTuple.Create(metricSetting.DisplayName, data.Result.Select(item => (double)((object[])item)[0]).ToArray(), data.Result.Select(item => (string)((object[])item)[1]).ToArray()));
                }
                break;
        }
        return result;
    }

    private string GetLabelName(object o, string defaultName)
    {
        var t = o as IDictionary<string, object>;
        if (!t.Any())
            return string.Empty;
        StringBuilder text = new StringBuilder();
        foreach (var key in t.Keys)
        {
            text.Append($"{key}:{t[key]},");
        }

        return text.Remove(text.Length - 1, 1).ToString();
    }

    private async Task SetChartType(string type, Func<Task<QueryResultDataResponse[]>> dataFn)
    {
        switch (type)
        {
            case "line":
                _type = EChartConst.Line;
                InitLine(await dataFn());
                break;
            case "bar":
                _type = EChartConst.Bar;
                InitBar(await dataFn());
                break;
            case "pie":
                _type = EChartConst.Pie;
                InitPie(await dataFn());
                break;
            case "gauge":
                _type = EChartConst.Gauge;
                InitGauge(await dataFn());
                break;
            case "heartmap":
                _type = EChartConst.Heatmap;
                InitHeatmap();
                break;
            case "line-area":
                _type = EChartConst.LineArea;
                InitLineArea(await dataFn());
                break;
            default:
                {
                    _options = new();
                    StateHasChanged();
                    break;
                }
        }
        StateHasChanged();
    }

    private async void OnTypeChange(string type)
    {
        _panelValue.ChartType = type;
        await SetChartType(type, LoadDataAsync);
    }

    private async Task OnItemsChange(List<EChartPanelMetricItemModel> data)
    {
        bool hasChange = HasMetricChange(_metrics, data);
        _metrics = data;
        var index = 1;
        _panelValue.Metrics.Clear();
        foreach (var item in _metrics)
        {
            _panelValue.Metrics.Add(new PanelMetricDto
            {
                Caculate = item.Caculate,
                Color = item.Color,
                Name = item.Name,
                Unit = item.Unit,
                Sort = index++
            });
        }
        if (hasChange)
            await SetChartType(_panelValue.ChartType, LoadDataAsync);
    }

    private bool HasMetricChange(List<EChartPanelMetricItemModel> data1, List<EChartPanelMetricItemModel> data2)
    {
        if (data1 == null || data2 == null)
            return true;
        if (data1.Count - data2.Count != 0)
            return true;
        return string.Join(',', data1.Select(item => item.Caculate)) != string.Join(',', data2.Select(item => item.Caculate));
    }

    private void InitLine(params QueryResultDataResponse[] data)
    {
        if (data == null || !data.Any())
        {
            return;
        }
        if (data.Any(t => t.ResultType != ResultTypes.Matrix))
        {
            //数据格式不支持
        }

        var list = new List<object>();
        var titles = new List<string>();
        var xPoints = new List<string>();

        foreach (var item in data)
        {
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric, default!);
                var timeSpans = sss.Values.Select(it => (double)it[0]).ToArray();
                var values = sss.Values.Select(it => (string)it[1]).ToArray();
                titles.Add(title);
                if (!xPoints.Any())
                {
                    xPoints = FmtTimespan(timeSpans);
                    _type.SetValue("xAxis.data", xPoints);
                }

                list.Add(new { name = title, type = "line", stack = "Total", data = values });
            }
        }

        _type.SetValue("legend.data", titles);
        _type.SetValue("series", list);
        _options = _type.Option;
    }

    private void InitBar(params QueryResultDataResponse[] data)
    {
        if (data == null || !data.Any())
        {
            return;
        }
        if (data.Any(t => t.ResultType != ResultTypes.Matrix))
        {
            //数据格式不支持
        }

        var list = new List<string>();
        var titles = new List<string>();

        foreach (var item in data)
        {
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric, default!);
                var timeSpans = sss.Values.Select(it => (double)it[0]).ToArray();
                if (timeSpans.Length - 20 <= 0)
                {
                    //考虑柱状分组
                }
                var values = sss.Values.Select(it => (string)it[1]).Last();
                titles.Add(title);
                list.Add(values);
            }
        }

        _type.SetValue("xAxis.data", titles);
        _type.SetValue("series[0].data", list);
        _options = _type.Option;
    }

    private void InitPie(params QueryResultDataResponse[] data)
    {
        if (data == null || !data.Any())
        {
            return;
        }
        if (data.Any(t => t.ResultType != ResultTypes.Matrix))
        {
            //数据格式不支持
        }

        var list = new List<object>();

        foreach (var item in data)
        {
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();

            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric, default!);
                //var timeSpans = sss.Values.Select(it => (double)it[0]).ToArray();
                //if (timeSpans.Length - 20 <= 0)
                //{
                //    //考虑柱状分组
                //}
                var value = sss.Values.Select(it => (string)it[1]).Last();
                list.Add(new { value, name = title });
            }
        }
        _type.SetValue("series[0].data", list);
        _options = _type.Option;
    }
    private void InitGauge(params QueryResultDataResponse[] data)
    {
        if (data == null || !data.Any())
        {
            return;
        }
        if (data.Any(t => t.ResultType != ResultTypes.Matrix))
        {
            //数据格式不支持
        }

        var list = new List<object>();

        foreach (var item in data)
        {
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();

            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric, default!);
                var value = sss.Values.Select(it => (string)it[1]).Last();
                _type.SetValue("series[0].data[0]", new { name = title, value });
                break;
            }
            break;
        }

        _options = _type.Option;
    }
    private void InitHeatmap(params QueryResultDataResponse[] data) { }
    private void InitLineArea(params QueryResultDataResponse[] data)
    {
        if (data == null || !data.Any())
        {
            return;
        }
        if (data.Any(t => t.ResultType != ResultTypes.Matrix))
        {
            //数据格式不支持
        }

        var list = new List<object>();
        var titles = new List<string>();
        var xPoints = new List<string>();

        foreach (var item in data)
        {
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric, default!);
                var timeSpans = sss.Values.Select(it => (double)it[0]).ToArray();
                var values = sss.Values.Select(it => (string)it[1]).ToArray();
                titles.Add(title);
                if (!xPoints.Any())
                {
                    xPoints = FmtTimespan(timeSpans);
                    _type.SetValue("xAxis.data", xPoints);
                }

                list.Add(new { name = title, type = "line", stack = "Total", areaStyle = new { }, emphasis = new { focus = "series" }, data = values });
            }
        }
        _type.SetValue("legend.data", titles);
        _type.SetValue("series", list);
        _options = _type.Option;
    }

    private List<string> FmtTimespan(double[] timeSpans)
    {
        var step = timeSpans[1] - timeSpans[0];
        int level = GetLevel((int)Math.Floor(step));
        var fmt = "HH:mm:ss";

        return timeSpans.Select(item => new DateTime((long)Math.Floor(item * 1000)).Format(CurrentTimeZone, fmt)).ToList();
    }

    private int GetLevel(int seconds)
    {
        return 0;
    }
}