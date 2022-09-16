// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartLineDataSymbolOption
{
    /// <summary>
    /// last element must has value "arrow"
    /// </summary>
    public string Symbol { get; set; }

    public int? SymbolSize { get; set; }

    public int? SymbolRotate { get; set; }

    public string Value { get; set; }

    public EChartPieBorderRadiusOptionLabel Label { get; set; }
}