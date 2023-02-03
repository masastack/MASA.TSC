// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

internal class EChartPieBorderRadiusOptionSerie
{
    public string Name { get; set; }

    public string Type { get; set; }

    public string[] Radius { get; set; }

    public bool? AvoidLabelOverlap { get; set; }

    public EChartPieBorderRadiusOptionSerieItemSyle ItemStyle { get; set; }

    public EChartPieBorderRadiusOptionLabel Label { get; set; }

    public EChartPieBorderRadiusOptionEmphasis Emphasis { get; set; }

    public IEnumerable<EChartOptionSerieData> Data { get; set; }
}

internal class EChartPieBorderRadiusOptionSerieItemSyle
{
    public int BorderRadius { get; set; }

    public string BorderColor { get; set; }

    public int BorderWidth { get; set; }
}

internal class EChartPieBorderRadiusOptionLabel
{
    public bool Show { get; set; }

    public string Position { get; set; }

    public string Formatter { get; set; }

    public string FontSize { get; set; }

    public string FontWeight { get; set; }
}

internal class EChartPieBorderRadiusOptionEmphasis
{
    public EChartPieBorderRadiusOptionEmphasisLabel Label { get; set; }
}

internal class EChartPieBorderRadiusOptionEmphasisLabel
{
    public bool Show { get; set; }

    public string Formatter { get; set; }

    public string FontSize { get; set; }

    public string FontWeight { get; set; }
}