// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public class UpsertChartPanelDto : UpsertPanelDto, ITopListPanelValue, ITablePanelValue, IEChartPanelValue
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

    public string ChartType
    {
        get
        {
            var value = this[ExtensionFieldTypes.ChartType];
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? "table";
            }
            return "table";
        }
        set
        {
            this[ExtensionFieldTypes.ChartType] = value;
            IsLoadChartData = true;
            IsLoadOption = true;
            switch (value)
            {
                case "line":
                    EChartType = EChartConst.Line;
                    break;
                case "bar":
                    EChartType = EChartConst.Bar;
                    break;
                case "pie":
                    EChartType = EChartConst.Pie;
                    break;
                case "gauge":
                    EChartType = EChartConst.Gauge;
                    break;
                case "heatmap":
                    EChartType = EChartConst.Heatmap;
                    break;
                case "line-area":
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

    public EChartType _eChartType;

    public EChartType EChartType
    {
        get => _eChartType ??= EChartConst.Line;
        set => _eChartType = value;
    }

    static JsonSerializerOptions JsonOption = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public UpsertChartPanelDto(Guid id)
    {
        Id = id;
        Metrics = new()
        {
            new()
        };
        PanelType = PanelTypes.Chart;
    }

    bool IsLoadChartData { get; set; }

    bool IsLoadOption { get; set; } = true;

    string Key { get; set; }

    List<QueryResultDataResponse> _chartData = new();
    List<List<Dessert>> _tableData = new();

    public void SetChartData(List<QueryResultDataResponse> chartData)
    {
        _chartData = chartData;
        IsLoadChartData = true;
    }

    public string GetChartKey()
    {
        return Key; //IsLoadChartData + ChartType;
    }

    public object GetChartOption()
    {
        LoadChartData();
        LoadChartOption();

        return EChartType.Json;
    }

    void LoadChartData()
    {
        if (IsLoadChartData is false) return;

        IsLoadChartData = false;
        if (ChartType is "line" or "bar")
        {
            var data = GetMatrixRangeData();
            EChartType.Json["series"] = new JsonArray(data.Take(3).Select(item => new JsonObject
            {
                ["type"] = ChartType,
                ["name"] = string.Join("-", item.Metric.Values),
                ["data"] = new JsonArray(item.Values.Take(1000).Select(value => new JsonArray(DateTimeOffset.FromUnixTimeSeconds((long)value[0]).DateTime.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDouble(value[1]))).ToArray())
            }).ToArray());
        }
        else if (ChartType is "line-area")
        {
            var data = GetMatrixRangeData();
            EChartType.Json["series"] = new JsonArray(data.Take(3).Select(item => new JsonObject
            {
                ["name"] = string.Join("-", item.Metric.Values),
                ["type"] = "line",
                ["stack"] = "Total",
                ["areaStyle"] = new JsonObject(),
                ["emphasis"] = new JsonObject()
                {
                    ["focus"] = "series"
                },
                ["data"] = new JsonArray(item.Values.Take(1000).Select(value => new JsonArray(DateTimeOffset.FromUnixTimeSeconds((long)value[0]).DateTime.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDouble(value[1]))).ToArray())
            }).ToArray());
        }
        else if (ChartType is "pie")
        {
            var data = GetInstantVectorData();
            EChartType.Json["series"].AsArray().First()["data"] = new JsonArray(data.Select(item => new JsonObject
            {
                ["name"] = string.Join("-", item.Metric.Values),
                ["value"] = Convert.ToDouble(item.Value[1])
            }).ToArray());
        }
        else if (ChartType is "gauge")
        {
            var data = GetInstantVectorData();
            EChartType.Json["series"].AsArray().First()["data"] = new JsonArray(data.Take(1).Select(item => new JsonObject
            {
                ["name"] = string.Join("-", item.Metric.Values),
                ["value"] = Convert.ToDouble(item.Value[1]),
                ["title"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray($"{GetPosition(data.IndexOf(item) + 1)}%", "80%")
                },
                ["detail"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray($"{GetPosition(data.IndexOf(item) + 1)}%", "95%")
                },
            }).ToArray());
        }
        else if (ChartType is "heatmap")
        {
            EChartType.Json["series"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["type"] = ChartType,
                ["data"] = new JsonArray(new JsonArray(8, 0, 0), new JsonArray(11, 0, 2), new JsonArray(15, 0, 3), new JsonArray(11, 11, 11), new JsonArray(10, 3, 5))
            }).ToArray());
        }

        int GetPosition(int index)
        {
            if (Metrics.Count == 1) return 0;
            else
            {
                return 60 * index - 32 * Metrics.Count;
            }
        }
    }

    List<QueryResultMatrixRangeResponse> GetMatrixRangeData()
    {
        List<QueryResultMatrixRangeResponse> data = new();

        if (_chartData is not null)
        {
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    foreach (var result in item.Result)
                    {
                        if (result is QueryResultMatrixRangeResponse matrix) data.Add(matrix);
                    }
                }
            }
        }

        return data;
    }

    List<QueryResultInstantVectorResponse> GetInstantVectorData()
    {
        List<QueryResultInstantVectorResponse> data = new();

        if (_chartData is not null)
        {
            foreach (var item in _chartData)
            {
                if (item is not null)
                {
                    foreach (var result in item.Result)
                    {
                        if (result is QueryResultInstantVectorResponse matrix) data.Add(matrix);
                    }
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
                    data.Add(item.Result.Select(item => (QueryResultMatrixRangeResponse)item).ToList());
                }
            }
        }

        return data;
    }

    void LoadChartOption()
    {
        if (IsLoadOption is false) return;

        IsLoadOption = false;

        if (ChartType is "line-area")
        {
            var yAxis = EChartType.Json["yAxis"].AsArray().First()!;
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
            var xAxis = EChartType.Json["xAxis"].AsArray().First()!;
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
        else
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
        EChartType.SetValue("legend.show", Legend.Show);
        EChartType.SetValue("legend.orient", Legend.Orient);
        EChartType.SetValue("legend.left", Legend.XPositon);
        EChartType.SetValue("legend.top", Legend.YPositon);
        EChartType.SetValue("legend.type", Legend.Type);
        EChartType.SetValue("tooltip.show", Tooltip.Show);
        EChartType.SetValue("tooltip.renderMode", Tooltip.RenderModel);
        EChartType.SetValue("tooltip.className", Tooltip.ClassName);
        EChartType.SetValue("tooltip.trigger", Tooltip.Trigger);
    }

    public List<List<Dessert>> GetTableOption()
    {
        return _tableData;
    }

    public void SetTableOption(List<string> services, string jumpName,string jumpId)
    {
        _tableData.Clear();
        var data = GetTableMatrixRangeData();
        foreach (var service in services)
        {
            var rowData = new List<Dessert>();
            rowData.Add(new Dessert { JumpId = jumpId, Text = service });
            rowData.AddRange(data.Select(item =>
            {
                var firstData = item.FirstOrDefault(e => 
                {
                    if(e.Metric?.TryGetValue(jumpName, out object serviceName) is true)
                    {
                        return serviceName.ToString() == service;
                    }
                    return false;
                });
                if (firstData is not null)
                {
                    return new Dessert { Text = firstData.Values.FirstOrDefault()?[1].ToString()??"" };
                }
                return new Dessert { Text = "" };
            }));
            _tableData.Add(rowData);
        }
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

    public override UpsertPanelDto Clone(UpsertPanelDto panel)
    {
        base.Clone(panel);
        var value = this[ExtensionFieldTypes.ChartType];
        if (value is string stringValue)
        {
            ChartType = stringValue;
        }
        else if (value is JsonElement jsonElement)
        {
            ChartType = jsonElement.GetString() ?? "table";
        }

        Tooltip.PropertyChanged += Tooltip_PropertyChanged;
        Legend.PropertyChanged += Legend_PropertyChanged;
        Toolbox.PropertyChanged += Toolbox_PropertyChanged;
        XAxis.PropertyChanged += XAxis_PropertyChanged;
        YAxis.PropertyChanged += YAxis_PropertyChanged;

        return this;
    }
}
