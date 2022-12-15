// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetChart : TscWidgetBase
{
    private EChartPanelDto _panelValue = new();
    private List<EChartPanelMetricItemModel> _echartMetrics = new();
    private List<StringNumber> _panelValues = new List<StringNumber>() { 1 };
    private Type _showType = typeof(Table);
    private Type _panelType = typeof(TableExpansionPanel);
    private Dictionary<string, object> _componentMetadata = new();
    private DynamicComponent _dynamicComponent;
    private TableOption _tableOption = new();
    private TopListOption _topListOption = new();

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
                _echartMetrics = _panelValue.Metrics.Select(x => new EChartPanelMetricItemModel
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
                    _echartMetrics = new List<EChartPanelMetricItemModel>();
                else
                    _echartMetrics.Clear();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await SetChartType(_panelValue.ChartType, LoadDataAsync);
        _tableOption.PropertyChanged += TableOption_PropertyChanged;
        _topListOption.PropertyChanged += TopListOption_PropertyChanged;
        await base.OnInitializedAsync();
    }

    private void TopListOption_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        (_dynamicComponent.Instance as TopList)?.StateUpdated();
    }

    private void TableOption_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        (_dynamicComponent.Instance as Table)?.StateUpdated();
    }

    private void FeatureChanged(List<StringNumber> features)
    {
        EChartOption.Toolbox.Feature = features;
    }

    private async Task<QueryResultDataResponse[]> LoadDataAsync()
    {
        if (_echartMetrics != null && _echartMetrics.Any())
        {
            var requestData = new List<QueryResultDataResponse>();
            foreach (var metric in _echartMetrics)
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

    private static string GetLabelName(object o)
    {
        if (o is not IDictionary<string, object> dic || !dic.Any())
            return string.Empty;
        StringBuilder text = new StringBuilder();
        foreach (var key in dic.Keys)
        {
            text.Append($"{key}:{dic[key]},");
        }

        return text.Remove(text.Length - 1, 1).ToString();
    }

    private async Task SetChartType(string type, Func<Task<QueryResultDataResponse[]>> dataFn)
    {
        switch (type)
        {
            case "line":
                EChartOption.EChartType = EChartConst.Line;
                InitLine(await dataFn());
                break;
            case "bar":
                EChartOption.EChartType = EChartConst.Bar;
                InitBar(await dataFn());
                break;
            case "pie":
                EChartOption.EChartType = EChartConst.Pie;
                InitPie(await dataFn());
                break;
            case "gauge":
                EChartOption.EChartType = EChartConst.Gauge;
                InitGauge(await dataFn());
                break;
            case "heatmap":
                EChartOption.EChartType = EChartConst.Heatmap;
                InitHeatmap();
                break;
            case "line-area":
                EChartOption.EChartType = EChartConst.LineArea;
                InitLineArea(await dataFn());
                break;
            default:
                break;
        }
        StateHasChanged();
    }

    private async void OnTypeChange(string type)
    {
        if (type == "table")
        {
            _panelType = typeof(TableExpansionPanel);
            _showType = typeof(Table);
            _componentMetadata = new Dictionary<string, object>{
                { "Title",_panelValue.Title},
                { "SystemIdentity",_panelValue.SystemIdentity},
                { "Option",_tableOption}
            };
        }
        else if (type == "top-list")
        {
            _panelType = typeof(TopListExpansionPanel);
            _showType = typeof(TopList);
            _componentMetadata = new Dictionary<string, object>{
                { "Title",_panelValue.Title},
                { "SystemIdentity",_panelValue.SystemIdentity},
                { "Option",_topListOption}
            };
        }
        else
        {
            _panelType = typeof(EChartExpansionPanel);
            _showType = typeof(EChart);
            _componentMetadata = new Dictionary<string, object>{
                { "Title",_panelValue.Title},
                { "Description",_panelValue.Description},
                { "Metrics",_echartMetrics },
                { "MetricsChanged",EventCallback.Factory.Create<List<EChartPanelMetricItemModel>>(this,OnItemsChange) }
            };
        }
        _panelValue.ChartType = type;
        await SetChartType(type, LoadDataAsync);
    }

    private async Task OnItemsChange(List<EChartPanelMetricItemModel> data)
    {
        //this method not work list add or delete
        bool hasChange = HasMetricChange(_echartMetrics, data);
        _echartMetrics = data;
        var index = 1;
        _panelValue.Metrics.Clear();
        foreach (var item in _echartMetrics)
        {
            _panelValue.Metrics.Add(new PanelMetricDto
            {
                Id = item.Id,
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

    private static bool HasMetricChange(List<EChartPanelMetricItemModel> data1, List<EChartPanelMetricItemModel> data2)
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
            if (item.Result == null || !item.Result.Any())
                continue;
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric!);
                var timeSpans = sss.Values!.Select(it => (double)it[0]).ToArray();
                var values = sss.Values!.Select(it => (string)it[1]).ToArray();
                titles.Add(title);
                if (!xPoints.Any())
                {
                    xPoints = FmtTimespan(timeSpans);
                    EChartOption.EChartType.SetValue("xAxis.data", xPoints);
                }

                list.Add(new { name = title, type = "line", stack = "Total", data = values });
            }
        }

        EChartOption.EChartType.SetValue("legend.data", titles);
        EChartOption.EChartType.SetValue("series", list);
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
            if (item.Result == null || !item.Result.Any())
                continue;
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric!);
                var timeSpans = sss.Values!.Select(it => (double)it[0]).ToArray();
                if (timeSpans.Length - 20 <= 0)
                {
                    //考虑柱状分组
                }
                var values = sss.Values!.Select(it => (string)it[1]).Last();
                titles.Add(title);
                list.Add(values);
            }
        }

        EChartOption.EChartType.SetValue("xAxis.data", titles);
        EChartOption.EChartType.SetValue("series[0].data", list);
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
            if (item.Result == null || !item.Result.Any())
                continue;
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();

            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric!);
                //var timeSpans = sss.Values.Select(it => (double)it[0]).ToArray();
                //if (timeSpans.Length - 20 <= 0)
                //{
                //    //考虑柱状分组
                //}
                var value = sss.Values!.Select(it => (string)it[1]).Last();
                list.Add(new { value, name = title });
            }
        }
        EChartOption.EChartType.SetValue("series[0].data", list);
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
            if (item.Result == null || !item.Result.Any())
                continue;
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();

            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric!);
                var value = sss.Values!.Select(it => (string)it[1]).Last();
                EChartOption.EChartType.SetValue("series[0].data[0]", new { name = title, value });
                break;
            }
            break;
        }
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
            if (item.Result == null || !item.Result.Any())
                continue;
            var matrixDatas = item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToArray();
            foreach (var sss in matrixDatas)
            {
                var title = GetLabelName(sss.Metric!);
                var timeSpans = sss.Values!.Select(it => (double)it[0]).ToArray();
                var values = sss.Values!.Select(it => (string)it[1]).ToArray();
                titles.Add(title);
                if (!xPoints.Any())
                {
                    xPoints = FmtTimespan(timeSpans);
                    EChartOption.EChartType.SetValue("xAxis.data", xPoints);
                }

                list.Add(new { name = title, type = "line", stack = "Total", areaStyle = new { }, emphasis = new { focus = "series" }, data = values });
            }
        }
        EChartOption.EChartType.SetValue("legend.data", titles);
        EChartOption.EChartType.SetValue("series", list);
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

    protected override void Dispose(bool disposing)
    {
        _tableOption.PropertyChanged -= TableOption_PropertyChanged;
        _topListOption.PropertyChanged -= TopListOption_PropertyChanged;
        base.Dispose(disposing);
    }
}