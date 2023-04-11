// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart.Models;

public class UpsertChartPanelDto : UpsertPanelDto, ITablePanelValue, IEChartPanelValue
{
    public string Color
    {
        get
        {
            var value = this[ExtensionFieldTypes.Color];
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? "";
            }
            return "";
        }
        set => this[ExtensionFieldTypes.Color] = value;
    }

    public string SystemIdentity
    {
        get
        {
            var value = this[ExtensionFieldTypes.SystemIdentity];
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? "";
            }
            return "";
        }
        set => this[ExtensionFieldTypes.SystemIdentity] = value;
    }

    public string Desc
    {
        get
        {
            var value = this[ExtensionFieldTypes.Desc];
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? "";
            }
            return "";
        }
        set => this[ExtensionFieldTypes.Desc] = value;
    }

    public int MaxCount
    {
        get
        {
            var value = this[ExtensionFieldTypes.MaxCount];
            if (value is int intValue)
            {
                return intValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetInt32();
            }
            return 3;
        }
        set => this[ExtensionFieldTypes.MaxCount] = value;
    }

    public ChartTypes ChartType
    {
        get
        {
            var value = this[ExtensionFieldTypes.ChartType];
            if (value is string strValue)
            {
                Enum.TryParse<ChartTypes>(strValue, out var convertValue);
                return convertValue;
            }
            else if (value is JsonElement jsonElement)
            {
                Enum.TryParse<ChartTypes>(jsonElement.ToString(), out var convertValue);
                return convertValue;
            }
            return ChartTypes.Table;
        }
        set
        {
            this[ExtensionFieldTypes.ChartType] = value.ToString();
            IsLoadChartData = true;
            IsLoadOption = true;
            switch (value)
            {
                case ChartTypes.Line:
                    EChartType = EChartConst.Line;
                    break;
                case ChartTypes.Bar:
                    EChartType = EChartConst.Bar;
                    break;
                case ChartTypes.Pie:
                    EChartType = EChartConst.Pie;
                    break;
                case ChartTypes.Gauge:
                    EChartType = EChartConst.Gauge;
                    break;
                case ChartTypes.Heatmap:
                    EChartType = EChartConst.Heatmap;
                    break;
                case ChartTypes.LineArea:
                    EChartType = EChartConst.LineArea;
                    break;
                default:
                    break;
            }
        }
    }

    public int ItemsPerPage
    {
        get
        {
            var value = this[ExtensionFieldTypes.ItemsPerPage];
            if (value is int intValue)
            {
                return intValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetInt32();
            }
            return 10;
        }
        set => this[ExtensionFieldTypes.ItemsPerPage] = value;
    }

    public bool ShowTableHeader
    {
        get
        {
            var value = this[ExtensionFieldTypes.ShowTableHeader];
            if (value is bool booleanValue)
            {
                return booleanValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetBoolean();
            }
            return true;
        }
        set => this[ExtensionFieldTypes.ShowTableHeader] = value;
    }

    public bool ShowTableFooter
    {
        get
        {
            var value = this[ExtensionFieldTypes.ShowTableFooter];
            if (value is bool booleanValue)
            {
                return booleanValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetBoolean();
            }
            return false;
        }
        set => this[ExtensionFieldTypes.ShowTableFooter] = value;
    }

    public bool EnablePaginaton
    {
        get
        {
            var value = this[ExtensionFieldTypes.EnablePaginaton];
            if (value is bool booleanValue)
            {
                return booleanValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetBoolean();
            }
            return false;
        }
        set => this[ExtensionFieldTypes.EnablePaginaton] = value;
    }

    public string ColumnAlignment
    {
        get
        {
            var value = this[ExtensionFieldTypes.ColumnAlignment];
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? "";
            }
            return "";
        }
        set => this[ExtensionFieldTypes.ColumnAlignment] = value;
    }

    public ListTypes ListType
    {
        get
        {
            var value = this[ExtensionFieldTypes.ListType];
            if (value is ListTypes enumValue)
            {
                return enumValue;
            }
            else if (value is JsonElement jsonElement)
            {
                var number = jsonElement.GetInt32();
                return (ListTypes)number;
            }
            return ListTypes.ServiceList;
        }
        set => this[ExtensionFieldTypes.ListType] = value;
    }

    public Tooltip Tooltip
    {
        get
        {
            var value = this[ExtensionFieldTypes.Tooltip];
            if (value is Tooltip tooltip)
            {
                return tooltip;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<Tooltip>(JsonOption);
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.Tooltip] = jsonValue;
                    return jsonValue;
                }
            }

            value = new Tooltip();
            this[ExtensionFieldTypes.Tooltip] = value;
            return (Tooltip)value;
        }
        set => this[ExtensionFieldTypes.Tooltip] = value;
    }

    public Legend Legend
    {
        get
        {
            var value = this[ExtensionFieldTypes.Legend];
            if (value is Legend legend)
            {
                return legend;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<Legend>(JsonOption);
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.Legend] = jsonValue;
                    return jsonValue;
                }
            }

            value = new Legend();
            this[ExtensionFieldTypes.Legend] = value;
            return (Legend)value;
        }
        set => this[ExtensionFieldTypes.Legend] = value;
    }

    public Toolbox Toolbox
    {
        get
        {
            var value = this[ExtensionFieldTypes.Toolbox];
            if (value is Toolbox toolbox)
            {
                return toolbox;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<Toolbox>(JsonOption);
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.Toolbox] = jsonValue;
                    return jsonValue;
                }
            }

            value = new Toolbox();
            this[ExtensionFieldTypes.Toolbox] = value;
            return (Toolbox)value;
        }
        set => this[ExtensionFieldTypes.Toolbox] = value;
    }

    public Axis XAxis
    {
        get
        {
            var value = this[ExtensionFieldTypes.XAxis];
            if (value is Axis axis)
            {
                return axis;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<Axis>(JsonOption);
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.XAxis] = jsonValue;
                    return jsonValue;
                }
            }

            value = new Axis();
            this[ExtensionFieldTypes.XAxis] = value;
            return (Axis)value;
        }
        set => this[ExtensionFieldTypes.XAxis] = value;
    }

    public Axis YAxis
    {
        get
        {
            var value = this[ExtensionFieldTypes.YAxis];
            if (value is Axis axis)
            {
                return axis;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<Axis>(JsonOption);
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.YAxis] = jsonValue;
                    return jsonValue;
                }
            }

            value = new Axis();
            this[ExtensionFieldTypes.YAxis] = value;
            return (Axis)value;
        }
        set => this[ExtensionFieldTypes.YAxis] = value;
    }

    private EChartType _eChartType;

    public EChartType EChartType
    {
        get => _eChartType ??= EChartConst.Line;
        set => _eChartType = value;
    }

    public Func<TopListOption, Task>? TopListOnclick { get; set; }

    static JsonSerializerOptions JsonOption = new()
    {
        PropertyNameCaseInsensitive = true
    };

    bool IsLoadChartData { get; set; }

    bool IsLoadOption { get; set; } = true;

    string Key { get; set; }

    string DateFormart { get; set; }

    List<QueryResultDataResponse> _chartData = new();

    readonly List<List<Dessert>> _tableData = new();

    readonly List<TopListOption> _topListData = new();

    string ToFormatTimeSpan(long timestamp, TimeZoneInfo timeZoneInfo)
    {
        return timestamp.ToDateTime(timeZoneInfo).Format(DateFormart);
    }

    public void SetChartData(List<QueryResultDataResponse> chartData, DateTime start, DateTime end)
    {
        DateFormart = start.Format(end);
        _chartData = chartData;
        IsLoadChartData = true;
        Key = "DataChanged" + Guid.NewGuid();
    }

    public void ReloadChartData()
    {
        if (ChartType is not ChartTypes.Table)
        {
            IsLoadChartData = true;
            IsLoadOption = true;
            Key = "ReloadChartData" + Guid.NewGuid();
        }
    }

    public string GetChartKey() => Key;

    public object? GetChartOption(TimeZoneInfo timeZoneInfo)
    {
        if (_chartData.Any(item => item?.Result?.Any() is true) is false) return null;
        LoadChartData(timeZoneInfo);
        LoadChartOption();

        return EChartType.Json;
    }

    public void SetTimeZoneChange()
    {
        IsLoadChartData = true;
        Key = "TimeZoneChange" + Guid.NewGuid();
    }

    void LoadChartData(TimeZoneInfo timeZoneInfo)
    {
        if (IsLoadChartData is false) return;
        IsLoadChartData = false;

        var data = GetMatrixRangeData();
        if (ChartType is ChartTypes.Line or ChartTypes.Bar)
        {
            EChartType.Json["series"] = new JsonArray(data.Select(item => new JsonObject
            {
                ["type"] = ChartType.ToString().ToLower(),
                ["name"] = item.Key.Name,
                ["color"] = item.Key.Color,
                ["data"] = new JsonArray(item.Value.Values!.Select(value => new JsonArray(ToFormatTimeSpan((long)value[0], timeZoneInfo), value[1].ToString())).ToArray())
            }).ToArray());
        }
        else if (ChartType is ChartTypes.LineArea)
        {
            EChartType.Json["series"] = new JsonArray(data.Select(item => new JsonObject
            {
                ["type"] = "line",
                ["name"] = item.Key.Name,
                ["color"] = item.Key.Color,
                ["stack"] = "Total",
                ["areaStyle"] = new JsonObject(),
                ["emphasis"] = new JsonObject()
                {
                    ["focus"] = "series"
                },
                ["data"] = new JsonArray(item.Value.Values!.Select(value => new JsonArray(ToFormatTimeSpan((long)value[0], timeZoneInfo), value[1].ToString())).ToArray())
            }).ToArray());
        }
        else if (ChartType is ChartTypes.Pie)
        {
            var serie = EChartType.Json["series"]!.AsArray().First()!;
            serie["color"] = new JsonArray(data.Select(item => (JsonNode)item.Key.Color).ToArray());
            serie["data"] = new JsonArray(data.Select(item => new JsonObject
            {
                ["name"] = item.Key.Name,
                ["value"] = GetQueryResultMatrixRangeResponseValue(item.Value)
            }).ToArray());
        }
        else if (ChartType is ChartTypes.Gauge)
        {
            var serie = EChartType.Json["series"]!.AsArray().First()!;
            serie["color"] = new JsonArray(data.Take(1).Select(item => (JsonNode)item.Key.Color).ToArray());
            serie["data"] = new JsonArray(data.Take(1).Select(item => new JsonObject
            {
                ["name"] = item.Key.Name,
                ["value"] = GetQueryResultMatrixRangeResponseValue(item.Value),
                ["title"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray("0%", "120%")
                },
                ["detail"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray("0%", "90%")
                },
            }).ToArray());
        }
        else if (ChartType is ChartTypes.Heatmap)
        {
            EChartType.Json["series"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["type"] = ChartType.ToString().ToLower(),
                ["data"] = new JsonArray(new JsonArray(8, 0, 0), new JsonArray(11, 0, 2), new JsonArray(15, 0, 3), new JsonArray(11, 11, 11), new JsonArray(10, 3, 5))
            }).ToArray());
        }
    }

    List<KeyValuePair<ChartSeriesOPtion, QueryResultMatrixRangeResponse>> GetMatrixRangeData()
    {
        List<KeyValuePair<ChartSeriesOPtion, QueryResultMatrixRangeResponse>> data = new();

        if (_chartData is not null)
        {
            var index = 0;
            var defaultColorIndex = 0;
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    var matrixs = item.Result?.Select(item => item as QueryResultMatrixRangeResponse)?.Where(item => item is not null)?.ToList() ?? new();
                    var multiple = matrixs.Count > 1;
                    foreach (var matrix in matrixs)
                    {
                        var metricName = "";
                        if (multiple)
                        {
                            metricName = string.Join("-", matrix.Metric!.Values);
                        }
                        else
                        {
                            metricName = Metrics[index].DisplayName ?? string.Join("-", matrix.Metric!.Values);
                        }
                        var color = Metrics[index].Color;
                        if (ChartType is ChartTypes.Pie or ChartTypes.Gauge)
                        {
                            if (string.IsNullOrEmpty(color))
                            {
                                color = _defaultColors[defaultColorIndex];
                                defaultColorIndex++;
                                if (defaultColorIndex == _defaultColors.Count())
                                {
                                    defaultColorIndex = 0;
                                }
                            }
                        }
                        data.Add(new(new()
                        {
                            Name = metricName,
                            Color = color,
                        }, matrix));
                    }
                }
                index++;
            }
        }

        return data;
    }

    List<KeyValuePair<ChartSeriesOPtion, QueryResultInstantVectorResponse>> GetInstantVectorData()
    {
        List<KeyValuePair<ChartSeriesOPtion, QueryResultInstantVectorResponse>> data = new();

        if (_chartData is not null)
        {
            var index = 0;
            var defaultColorIndex = 0;
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    var matrixs = item.Result?.Select(item => item as QueryResultInstantVectorResponse)?.Where(item => item is not null)?.ToList() ?? new();
                    var multiple = matrixs.Count > 1;
                    foreach (var matrix in matrixs)
                    {
                        var metricName = "";
                        if (multiple)
                        {
                            metricName = string.Join("-", matrix.Metric!.Values);
                        }
                        else
                        {
                            metricName = Metrics[index].DisplayName ?? string.Join("-", matrix.Metric!.Values);
                        }
                        var color = Metrics[index].Color;
                        if (string.IsNullOrEmpty(color))
                        {
                            color = _defaultColors[defaultColorIndex];
                            defaultColorIndex++;
                            if (defaultColorIndex == _defaultColors.Count())
                            {
                                defaultColorIndex = 0;
                            }
                        }
                        data.Add(new(new()
                        {
                            Name = metricName,
                            Color = color,
                        }, matrix));
                    }
                }
                index++;
            }
        }

        return data;
    }

    List<List<QueryResultInstantVectorResponse>> GetTableInstantVectorData()
    {
        List<List<QueryResultInstantVectorResponse>> data = new();

        if (_chartData is not null)
        {
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    var matrixs = new List<QueryResultInstantVectorResponse>();
                    foreach (var result in item.Result!)
                    {
                        if (result is QueryResultInstantVectorResponse matrix) matrixs.Add(matrix);
                    }
                    data.Add(matrixs);
                }
            }
        }

        return data;
    }

    List<List<QueryResultMatrixRangeResponse>> GetTableMatrixRangeData()
    {
        List<List<QueryResultMatrixRangeResponse>> data = new();

        if (_chartData is not null)
        {
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    var matrixs = new List<QueryResultMatrixRangeResponse>();
                    foreach (var result in item.Result!)
                    {
                        if (result is QueryResultMatrixRangeResponse matrix) matrixs.Add(matrix);
                    }
                    data.Add(matrixs);
                }
            }
        }

        return data;
    }

    string[] _defaultColors = new string[] { "#5470c6", "#91cc75", "#fac858", "#ee6666", "#73c0de", "#3ba272", "#fc8452", "#9a60b4", "#ea7ccc" };

    void LoadChartOption()
    {
        if (IsLoadOption is false) return;

        IsLoadOption = false;

        if (ChartType is ChartTypes.LineArea)
        {
            var yAxis = EChartType.Json["yAxis"]!.AsArray().First()!;
            yAxis["show"] = YAxis.Show;
            yAxis["axisLine"] = new JsonObject
            {
                ["show"] = YAxis.ShowLine
            };
            yAxis["axisTick"] = new JsonObject
            {
                ["show"] = YAxis.ShowTick
            };
            yAxis["axisLabel"] = new JsonObject
            {
                ["show"] = YAxis.ShowLabel
            };
            var xAxis = EChartType.Json["xAxis"]!.AsArray().First()!;
            xAxis["show"] = XAxis.Show;
            xAxis["axisLine"] = new JsonObject
            {
                ["show"] = XAxis.ShowLine
            };
            xAxis["axisTick"] = new JsonObject
            {
                ["show"] = XAxis.ShowTick
            };
            xAxis["axisLabel"] = new JsonObject
            {
                ["show"] = XAxis.ShowLabel
            };
        }
        else if (ChartType is not ChartTypes.Pie && ChartType is not ChartTypes.Gauge)
        {
            EChartType.SetValue("yAxis.show", YAxis.Show);
            EChartType.SetValue("yAxis.axisLine.show", YAxis.ShowLine);
            EChartType.SetValue("yAxis.axisTick.show", YAxis.ShowTick);
            EChartType.SetValue("yAxis.axisLabel.show", YAxis.ShowLabel);
            EChartType.SetValue("xAxis.show", XAxis.Show);
            EChartType.SetValue("xAxis.axisLine.show", XAxis.ShowLine);
            EChartType.SetValue("xAxis.axisTick.show", XAxis.ShowTick);
            EChartType.SetValue("xAxis.axisLabel.show", XAxis.ShowLabel);
        }

        EChartType.SetValue("toolbox.show", Toolbox.Show);
        EChartType.SetValue("toolbox.orient", Toolbox.Orient);
        EChartType.SetValue("toolbox.left", Toolbox.XPositon);
        EChartType.SetValue("toolbox.top", Toolbox.YPositon);
        EChartType.SetValue("toolbox.feature", Toolbox.Feature.ToDictionary(f => f.AsT0, f => new object()));

        if (ChartType is not ChartTypes.Gauge)
        {
            EChartType.SetValue("legend.show", Legend.Show);
            EChartType.SetValue("legend.orient", Legend.Orient);
            EChartType.SetValue("legend.left", Legend.XPositon);
            EChartType.SetValue("legend.top", Legend.YPositon);
            EChartType.SetValue("legend.type", Legend.Type);
            EChartType.SetValue("tooltip.show", Tooltip.Show);
        }

        EChartType.SetValue("tooltip.renderMode", Tooltip.RenderModel);
        EChartType.SetValue("tooltip.className", Tooltip.ClassName);
        EChartType.SetValue("tooltip.trigger", Tooltip.Trigger);
        EChartType.SetValue("tooltip.confine", true);

        EChartType.SetValue("color", _defaultColors);
        EChartType.SetValue("grid", new
        {
            x = 30,
            x2 = 10,
            y2 = 20,
            y = Metrics.Any(metric => string.IsNullOrEmpty(metric.DisplayName) is false) ? 40 : 10
        }); ;
    }

    public List<List<Dessert>> GetTableOption() => _tableData;

    public List<TopListOption> GetTopListOption() => _topListData;

    public void SetTableOption(List<string> services, string jumpName, string jumpId)
    {
        _tableData.Clear();
        var data = GetTableMatrixRangeData();
        if (services == null || !services.Any())
            return;
        foreach (var service in services)
        {
            var rowData = new List<Dessert>();
            rowData.Add(new Dessert { JumpId = jumpId, Text = service });
            rowData.AddRange(data.Select(item =>
            {
                var firstData = item.FirstOrDefault(e =>
                {
                    if (e.Metric?.TryGetValue(jumpName, out var serviceName) is true)
                    {
                        return serviceName.ToString() == service;
                    }
                    return false;
                });
                if (firstData is not null)
                {
                    var value = GetQueryResultMatrixRangeResponseValue(firstData).ToString();
                    return new Dessert { Text = value };
                }
                return new Dessert { Text = "" };
            }));
            _tableData.Add(rowData);
        }
    }

    public void SetTopListOption(string href)
    {
        _topListData.Clear();

        var data = GetTableMatrixRangeData().FirstOrDefault();
        if (data is null) return;
        _topListData.AddRange(data.Select(item => new TopListOption
        {
            Href = href,
            Text = string.Join('-', item.Metric!.Select(metric => metric.Value)),
            Value = GetQueryResultMatrixRangeResponseValue(item)
        }));
    }

    private double GetQueryResultMatrixRangeResponseValue(QueryResultMatrixRangeResponse item)
    {
        var values = item.Values.Select(ToDouble).Where(val => !double.IsNaN(val));
        if (values.Any())
            return values.Average().FloorDouble(2);
        return default;
    }

    private double ToDouble(object[] itemValue)
    {
        double val = Convert.ToDouble(itemValue[1]);
        return val;
    }

    private void YAxis_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Key = "YAxis" + Guid.NewGuid();
        IsLoadOption = true;
    }

    private void XAxis_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Key = "XAxis" + Guid.NewGuid();
        IsLoadOption = true;
    }

    private void Toolbox_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Key = "Toolbox" + Guid.NewGuid();
        IsLoadOption = true;
    }

    private void Legend_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Key = "Legend" + Guid.NewGuid();
        IsLoadOption = true;
    }

    private void Tooltip_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Key = "Tooltip" + Guid.NewGuid();
        IsLoadOption = true;
    }

    public UpsertChartPanelDto(Guid id) : base()
    {
        Id = id;
        Metrics = new()
        {
            new()
        };
        PanelType = PanelTypes.Chart;
        Tooltip.PropertyChanged += Tooltip_PropertyChanged;
        Legend.PropertyChanged += Legend_PropertyChanged;
        Toolbox.PropertyChanged += Toolbox_PropertyChanged;
        XAxis.PropertyChanged += XAxis_PropertyChanged;
        YAxis.PropertyChanged += YAxis_PropertyChanged;
    }

    public override UpsertPanelDto Clone(UpsertPanelDto panel)
    {
        base.Clone(panel);
        var value = this[ExtensionFieldTypes.ChartType];
        if (value is string strValue)
        {
            Enum.TryParse<ChartTypes>(strValue, out var chartType);
            ChartType = chartType;
        }
        else if (value is JsonElement jsonElement)
        {
            Enum.TryParse<ChartTypes>(jsonElement.ToString(), out var chartType);
            ChartType = chartType;
        }

        Tooltip.PropertyChanged += Tooltip_PropertyChanged;
        Legend.PropertyChanged += Legend_PropertyChanged;
        Toolbox.PropertyChanged += Toolbox_PropertyChanged;
        XAxis.PropertyChanged += XAxis_PropertyChanged;
        YAxis.PropertyChanged += YAxis_PropertyChanged;

        return this;
    }
}
