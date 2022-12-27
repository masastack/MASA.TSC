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
                    this[ExtensionFieldTypes.EChartType] = EChartConst.Line;
                    break;
                case "bar":
                    this[ExtensionFieldTypes.EChartType] = EChartConst.Bar;
                    break;
                case "pie":
                    this[ExtensionFieldTypes.EChartType] = EChartConst.Pie;
                    break;
                case "gauge":
                    this[ExtensionFieldTypes.EChartType] = EChartConst.Gauge;
                    break;
                case "heatmap":
                    this[ExtensionFieldTypes.EChartType] = EChartConst.Heatmap;
                    break;
                case "line-area":
                    this[ExtensionFieldTypes.EChartType] = EChartConst.LineArea;
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

    public EChartType EChartType
    {
        get
        {
            var value = this[ExtensionFieldTypes.EChartType];
            if (value is EChartType echartType)
            {
                return echartType;
            }
            else if (value is JsonElement jsonElement)
            {
                var jsonValue = jsonElement.Deserialize<EChartType>();
                if (jsonValue is not null)
                {
                    this[ExtensionFieldTypes.EChartType] = jsonValue;
                    return jsonValue;
                }
            }

            value = EChartConst.Line;
            this[ExtensionFieldTypes.EChartType] = value;
            return (EChartType)value;
        }
        set => this[ExtensionFieldTypes.EChartType] = value;
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

    //public UpsertPanelDto Clone(UpsertPanelDto panel)
    //{
    //    Id = panel.Id;
    //    Title = panel.Title;
    //    Description = panel.Description;
    //    PanelType = panel.PanelType;
    //    Width = panel.Width;
    //    Height = panel.Height;
    //    X = panel.X;
    //    Y = panel.Y;
    //    Metrics.Clear();
    //    Metrics.AddRange(panel.Metrics);
    //    ExtensionData.Clear();
    //    foreach (var (key, value) in panel.ExtensionData)
    //    {
    //        ExtensionData.Add(key, value);
    //    }

    //    return this;
    //}
}
