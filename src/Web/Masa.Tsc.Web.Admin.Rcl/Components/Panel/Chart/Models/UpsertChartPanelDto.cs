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
            return false;
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

    public EChartType _eChartType;

    public EChartType EChartType
    {
        get => _eChartType ??= EChartConst.Line;
        set => _eChartType = value;
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
                var jsonValue = jsonElement.Deserialize<Tooltip>();
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
                var jsonValue = jsonElement.Deserialize<Legend>();
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
                var jsonValue = jsonElement.Deserialize<Toolbox>();
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
                var jsonValue = jsonElement.Deserialize<Axis>();
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
                var jsonValue = jsonElement.Deserialize<Axis>();
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

    public UpsertChartPanelDto(Guid id)
    {
        Id = id;
        Metrics = new()
        {
            new()
        };
        PanelType = PanelTypes.Chart;
    }

    string Key { get; set; }

    List<QueryResultDataResponse> _chartData =new();

    string ChartDataKey { get; set; }

    public void SetChartData(List<QueryResultDataResponse> chartData)
    {
        _chartData = chartData;
        ChartDataKey = Guid.NewGuid().ToString();
    }

    public string GetChartKey()
    {
        return ChartDataKey + ChartType + string.Join(",", base.Metrics.Select(item => item.Name).Union(base.Metrics.Select(item => item.DisplayName)));
    }

    public object GetChartData()
    {
        var key = GetChartKey();
        if (Key == key) return EChartType.Json;
        Key = key;

        if(ChartType is "line" or "bar")
        {
            List<QueryResultMatrixRangeResponse> data = new();
            foreach(var item in _chartData)
            {
                if (item is not null)
                {
                    foreach(var result in item.Result)
                    {
                        if (result is QueryResultMatrixRangeResponse matrix) data.Add(matrix);
                    }
                }
            }
            EChartType.Json["series"] = new JsonArray(data.Take(3).Select(item => new JsonObject 
            {
                ["type"] = ChartType,
                ["name"] = string.Join("-",item.Metric.Values),
                ["data"] = new JsonArray(item.Values.Take(1000).Select(value => new JsonArray(DateTimeOffset.FromUnixTimeSeconds((long)value[0]).DateTime.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToInt32(value[1]))).ToArray())
            }).ToArray());
            //EChartType.Json["series"] = new JsonArray(Metrics.Select(item => new JsonObject
            //{
            //    ["name"] = item.DisplayName,
            //    ["type"] = ChartType,
            //    ["data"] = new JsonArray(120 + Random.Shared.Next(100), 200 + Random.Shared.Next(100), 150 + Random.Shared.Next(100), 80 + Random.Shared.Next(100), 70 + Random.Shared.Next(100), 110 + Random.Shared.Next(100), 130,145,54534,53453)
            //}).ToArray());
        }
        else if(ChartType is "pie")
        {
            EChartType.Json["series"].AsArray().First()["data"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["value"] = Random.Shared.Next(100)
            }).ToArray());
        }
        else if (ChartType is "line-area")
        {
            EChartType.Json["legend"]!["data"]= new JsonArray(Metrics.Select(item => (JsonNode)item.DisplayName!).ToArray());
            EChartType.Json["series"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["type"] = "line",
                ["stack"] = "Total",
                ["areaStyle"] = new JsonObject(),
                ["emphasis"] = new JsonObject() 
                {
                    ["focus"] = "series"
                },
                ["data"] = new JsonArray(120 + Random.Shared.Next(100), 200 + Random.Shared.Next(100), 150 + Random.Shared.Next(100), 80 + Random.Shared.Next(100), 70 + Random.Shared.Next(100), 110 + Random.Shared.Next(100), 130)
            }).ToArray());
        }
        else if(ChartType is "gauge")
        {
            EChartType.Json["series"].AsArray().First()["data"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["value"] = Random.Shared.Next(100),
                ["title"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray($"{GetPosition(Metrics.IndexOf(item)+1)}%","80%")
                },
                ["detail"] = new JsonObject()
                {
                    ["offsetCenter"] = new JsonArray($"{GetPosition(Metrics.IndexOf(item)+1)}%", "95%")
                },
            }).ToArray());
        }
        else if(ChartType is "heatmap")
        {
            EChartType.Json["series"] = new JsonArray(Metrics.Select(item => new JsonObject
            {
                ["name"] = item.DisplayName,
                ["type"] = ChartType,
                ["data"] = new JsonArray(new JsonArray(8,0,0), new JsonArray(11,0,2), new JsonArray(15,0,3), new JsonArray(11,11,11), new JsonArray(10,3,5))
            }).ToArray());
        }

        return EChartType.Json;

        int GetPosition(int index)
        {
            if (Metrics.Count == 1) return 0;
            else
            {
                return 60 * index - 32 * Metrics.Count;
            }
        }
    }
}
