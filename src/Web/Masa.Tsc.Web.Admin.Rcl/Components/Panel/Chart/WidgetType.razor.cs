// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart;

public partial class WidgetType
{
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string Value { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            (Items.FirstOrDefault(item => item.Type == Value) ?? Items.First()).Selected = true;
            base.StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSelected(EChartPanelTypeModel item)
    {
        Value = item.Type;
        Items.Where(x => x.Selected).ForEach(x => x.Selected = false);
        item.Selected = true;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }

    private List<EChartPanelTypeModel> Items { get; set; } = new List<EChartPanelTypeModel>
    {
        new EChartPanelTypeModel
        {
            Name="表格",
            Type="table",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/other/table.png"
        },
        new EChartPanelTypeModel
        {
            Name="折线图",
            Type="line",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/line.png"
        },
        new EChartPanelTypeModel
        {
            Name="柱形图",
            Type="bar",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/bar.png"
        },
        new EChartPanelTypeModel
        {
            Name="堆叠面积",
            Type="line-area",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/line-area.png"
        },
        new EChartPanelTypeModel
        {
            Name="饼图",
            Type="pie",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/pie.png"
        },
        new EChartPanelTypeModel
        {
            Name="仪表盘",
            Type="gauge",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/gauge.png"
        },
        new EChartPanelTypeModel
        {
            Name="热力图",
            Type="heatmap",
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/heatmap.png"
        }
    };
}
