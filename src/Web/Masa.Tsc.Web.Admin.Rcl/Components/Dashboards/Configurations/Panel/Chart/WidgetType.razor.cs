// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart;

public partial class WidgetType
{
    [Parameter]
    public EventCallback<ChartTypes> ValueChanged { get; set; }

    [Parameter]
    public ChartTypes Value { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            (Items.FirstOrDefault(item => item.Type == Value) ?? Items.First()).Selected = true;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSelected(EChartPanelTypeModel item)
    {
        Value = item.Type;
        Items.Where(x => x.Selected).ToList().ForEach(x => x.Selected = false);
        item.Selected = true;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }

    private List<EChartPanelTypeModel> Items { get; set; } = new List<EChartPanelTypeModel>
    {
        new EChartPanelTypeModel
        {
            Name="表格",
            Type= ChartTypes.Table,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/other/table.png"
        },
        new EChartPanelTypeModel
        {
            Name="折线图",
            Type=ChartTypes.Line,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/line.png"
        },
        new EChartPanelTypeModel
        {
            Name="柱形图",
            Type=ChartTypes.Bar,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/bar.png"
        },
        new EChartPanelTypeModel
        {
            Name="堆叠面积",
            Type=ChartTypes.LineArea,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/line-area.png"
        },
        new EChartPanelTypeModel
        {
            Name="饼图",
            Type=ChartTypes.Pie,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/pie.png"
        },
        new EChartPanelTypeModel
        {
            Name="仪表盘",
            Type=ChartTypes.Gauge,
            Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/gauge.png"
        },
        //new EChartPanelTypeModel
        //{
        //    Name="热力图",
        //    Type=ChartTypes.Heatmap,
        //    Src="_content/Masa.Tsc.Web.Admin.Rcl/img/echarts/heatmap.png"
        //}
    };
}
