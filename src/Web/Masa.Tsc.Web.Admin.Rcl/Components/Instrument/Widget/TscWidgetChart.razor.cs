// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Data.Prometheus.Enums;
using Masa.Utils.Data.Prometheus.Model;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetChart : TscWidgetBase
{
    private EChartPanelDto _panelValue = new();

    private List<EChartPanelMetricItemModel> _metrics = new();

    private object _options = new();

    private EChartType _type;

    private EChartLineOption _options1 = new()
    {
        Legend = new EChartOptionLegend
        {
            Data = new string[0],
            Orient = EchartOrientTypes.horizontal,
            Bottom = "1%"
        },
        Grid = new EChartOptionGrid
        {
            Left = "1%",
            Right = "2%",
            Top = "2%",
            Bottom = "20%",
            ContainLabel = true
        }
    };

    public override PanelDto Value
    {
        get => _panelValue;
        set
        {
            if (value is null) _panelValue = new()
            {
                Title = "tabs"
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
            //if (_panelValue.Tabs == null || !_panelValue.Tabs.Any())
            //{
            //    _panelValue.Tabs = new List<TabItemPanelDto>
            //        {
            //            new TabItemPanelDto{ Id=Guid.NewGuid(),Title="tab1",InstrumentId=_panelValue.InstrumentId,Sort=1 },
            //            new TabItemPanelDto{ Id=Guid.NewGuid(),Title="tab2",InstrumentId=_panelValue.InstrumentId,Sort=2 },
            //            new TabItemPanelDto{Id=Guid.NewGuid(),Title="tab3",InstrumentId=_panelValue.InstrumentId,Sort=3 }
            //        };
            //}
            //_value = _panelValue.Tabs[0].Id.ToString();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (_panelValue.Metrics == null)
            _panelValue.Metrics = new();

        _panelValue.Metrics.Add(new PanelMetricDto
        {
            Caculate = "sum by(job) (rate (process_resident_memory_bytes[30s]))/(2^20)",
            DisplayName = "process_resident_memory_bytes",
            Unit = ""

        });
        _panelValue.Metrics.Add(new PanelMetricDto
        {
            Caculate = "sum(up)",
            DisplayName = "s_upname",
            Unit = ""
        });
        SetChartType(_panelValue.ChartType);
        await LoadDataAsync();
        await base.OnInitializedAsync();
    }

    private async Task LoadDataAsync()
    {
        if (_panelValue.Metrics != null && _panelValue.Metrics.Any())
        {
            List<EChartLineOptionSerie> ddddd = new();
            int index = 0;
            bool hasX = false;
            List<string> names = new List<string>();
            foreach (var metric in _panelValue.Metrics)
            {
                var data = await ApiCaller.MetricService.GetQueryRangeAsync(new RequestMetricAggDto
                {
                    Match = metric.Caculate,
                    Start = DateTime.Now.Date.AddDays(-10),
                    End = DateTime.Now,
                    Step = "5m"
                });
                var convertData = asdasdasd(data, metric);
                foreach (var item in convertData)
                {
                    var name = string.IsNullOrEmpty(item.Item1) ? metric.DisplayName : item.Item1;
                    ddddd.Add(new EChartLineOptionSerie
                    {
                        Name = name,
                        Type = "line",
                        Stack = "Total",
                        Data = item.Item3
                    });
                    names.Add(name);
                    if (!hasX)
                    {
                        _options1.XAxis.Data = item.Item2.Select(item => new DateTime((long)Math.Floor(item * 1000)).Format(CurrentTimeZone, "HH:mm:ss")).ToArray();
                    }
                }

                //ddddd[index++] = new EChartLineOptionSerie
                //{
                //    Name = metric.DisplayName,
                //    Type = "line",
                //    Stack = metric.DisplayName,
                //    Data = convertData.Item2
                //};

            }
            _options1.Legend.Data = names;
            _options1.Series = ddddd.ToArray();
            _options = _options1;
            var tt = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var t = JsonSerializer.Serialize(_options, tt);
        }
        StateHasChanged();
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
                        result.Add(ValueTuple.Create(GetLabelName(item.Metric, metricSetting.DisplayName), item.Values.Select(it => (double)it[0]).ToArray(), item.Values.Select(it => (string)it[1]).ToArray()));
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

    private EChartType Get(string type)
    {
        switch (type)
        {
            case "line":
                return EChartConst.Line;
            default:
                return default!;
        }
    }

    private void SetChartType(string type)
    {
        switch (type)
        {
            case "line":
                _type = EChartConst.Line;
                InitLine();
                break;
            case "bar":
                _type = EChartConst.Bar;
                InitBar();
                break;
            case "pie":
                _type = EChartConst.Pie;
                InitPie();
                break;
            case "gauge":
                _type = EChartConst.Gauge;
                InitGauge();
                break;
            case "heartmap":
                _type = EChartConst.Heatmap;
                InitHeatmap();
                break;
            case "linearea":
                _type = EChartConst.LineArea;
                InitLineArea();
                break;
        }
    }

    private async void OnTypeChange(string type)
    {
        _panelValue.ChartType = type;
        SetChartType(type);
        await LoadDataAsync();
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

            }
        }


        //_type.SetValue("", null);
    }

    private void InitBar() { }
    private void InitPie() { }
    private void InitGauge() { }
    private void InitHeatmap() { }
    private void InitLineArea() { }
}