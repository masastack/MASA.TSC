// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public class UpsertChartPanelDto : UpsertPanelDto, ITopListPanelValue, ITablePanelValue, IEChartPanelValue
{
    public string Color
    {
        get => this[ExtensionFieldTypes.Color] as string ?? "";
        set => this[ExtensionFieldTypes.Color] = value;
    }

    public string SystemIdentity
    {
        get => this[ExtensionFieldTypes.SystemIdentity] as string ?? "";
        set => this[ExtensionFieldTypes.SystemIdentity] = value;
    }

    public string Desc
    {
        get => this[ExtensionFieldTypes.Desc] as string ?? "";
        set => this[ExtensionFieldTypes.Desc] = value;
    }

    public int MaxCount
    {
        get => this[ExtensionFieldTypes.MaxCount] as int? ?? 3;
        set => this[ExtensionFieldTypes.MaxCount] = value;
    }

    public string ChartType
    {
        get => this[ExtensionFieldTypes.ChartType] as string ?? "table";
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
        get => this[ExtensionFieldTypes.ItemsPerPage] as int? ?? 10;
        set => this[ExtensionFieldTypes.ItemsPerPage] = value;
    }

    public bool ShowTableHeader
    {
        get => this[ExtensionFieldTypes.ShowTableHeader] as bool? ?? false;
        set => this[ExtensionFieldTypes.ShowTableHeader] = value;
    }

    public bool ShowTableFooter
    {
        get => this[ExtensionFieldTypes.ShowTableFooter] as bool? ?? false;
        set => this[ExtensionFieldTypes.ShowTableFooter] = value;
    }

    public bool EnablePaginaton
    {
        get => this[ExtensionFieldTypes.EnablePaginaton] as bool? ?? false;
        set => this[ExtensionFieldTypes.EnablePaginaton] = value;
    }

    public string ColumnAlignment
    {
        get => this[ExtensionFieldTypes.ColumnAlignment] as string ?? "";
        set => this[ExtensionFieldTypes.ColumnAlignment] = value;
    }

    public EChartType EChartType
    {
        get
        {            
            var value = this[ExtensionFieldTypes.EChartType] as EChartType;
            if (value is null)
            {
                value = EChartConst.Line;
                this[ExtensionFieldTypes.EChartType] = value;
            }
            return value;
        }
        set => this[ExtensionFieldTypes.EChartType] = value;
    }

    public Tooltip Tooltip
    {
        get
        {
            var value = this[ExtensionFieldTypes.Tooltip] as Tooltip;
            if (value is null)
            {
                value = new();
                this[ExtensionFieldTypes.Tooltip] = value;
            }
            return value;
        }
        set => this[ExtensionFieldTypes.Tooltip] = value;
    }

    public Legend Legend
    {
        get
        {
            var value = this[ExtensionFieldTypes.Legend] as Legend;
            if (value is null)
            {
                value = new();
                this[ExtensionFieldTypes.Legend] = value;
            }
            return value;
        }
        set => this[ExtensionFieldTypes.Legend] = value;
    }

    public Toolbox Toolbox
    {
        get
        {
            var value = this[ExtensionFieldTypes.Toolbox] as Toolbox;
            if (value is null)
            {
                value = new();
                this[ExtensionFieldTypes.Toolbox] = value;
            }
            return value;
        }
        set => this[ExtensionFieldTypes.Toolbox] = value;
    }

    public Axis XAxis
    {
        get
        {
            var value = this[ExtensionFieldTypes.XAxis] as Axis;
            if (value is null)
            {
                value = new();
                this[ExtensionFieldTypes.XAxis] = value;
            }
            return value;
        }
        set => this[ExtensionFieldTypes.XAxis] = value;
    }

    public Axis YAxis
    {
        get
        {
            var value = this[ExtensionFieldTypes.YAxis] as Axis;
            if (value is null)
            {
                value = new();
                this[ExtensionFieldTypes.YAxis] = value;
            }
            return value;
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
}
