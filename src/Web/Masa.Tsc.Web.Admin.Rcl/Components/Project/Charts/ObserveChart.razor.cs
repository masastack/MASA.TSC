// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ObserveChart : TscEChartBase
{
    [Parameter]
    public StringNumber Width { get; set; } = "100%";

    [Parameter]
    public StringNumber Height { get; set; } = "100%";

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string SubText { get; set; }

    private EChartType _options = EChartConst.Pie;

    public int Total { get; set; } = 23;

    internal override async Task LoadAsync(ProjectAppSearchModel query)
    {
        _options.SetValue("legend", new { bottom = 10, left = "center" });
        _options.SetValue("title", new
        {
            text = "69%",
            x = "center",
            y = "center",
            textStyle = new
            {
                fontSize = 30
            }
        });
        _options.SetValue("series[0].radius", new List<string> { "40%", "70%" });
        _options.SetValue("series[0].label", new { show = false });
        await Task.CompletedTask;
    }
}