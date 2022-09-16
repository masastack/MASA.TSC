// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartPieBorderRadiusOptionSerie
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

public class EChartPieBorderRadiusOptionSerieItemSyle
{
    public int BorderRadius { get; set; }

    public string BorderColor { get; set; }

    public int BorderWidth { get; set; }
}

public class EChartPieBorderRadiusOptionLabel
{
    public bool Show { get; set; }

    public string Position { get; set; }

    public string Formatter { get; set; }

    public string FontSize { get; set; }

    public string FontWeight { get; set; }
}

public class EChartPieBorderRadiusOptionEmphasis
{
    public EChartPieBorderRadiusOptionEmphasisLabel Label { get; set; }
}

public class EChartPieBorderRadiusOptionEmphasisLabel
{
    public bool Show { get; set; }

    public string Formatter { get; set; }

    public string FontSize { get; set; }

    public string FontWeight { get; set; }
}