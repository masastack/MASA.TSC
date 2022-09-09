// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartOptionSerie
{
    public string Name { get; set; }

    public string Type { get; set; } = "pie";

    public string Radius { get; set; }

    public IEnumerable<EChartOptionSerieData> Data { get; set; }

    public EChartOptionSerieEmphasis Emphasis { get; set; }

    public EChartPieBorderRadiusOptionLabel Label { get; set; }
}

public class EChartOptionSerieEmphasis
{
    public EChartOptionItemStyle ItemStyle { get; set; } = new EChartOptionItemStyle
    {
        ShadowBlur = 10,
        ShadowOffsetX = 0,
        ShadowColor = "rgba(0, 0, 0, 0.5)"
    };
}