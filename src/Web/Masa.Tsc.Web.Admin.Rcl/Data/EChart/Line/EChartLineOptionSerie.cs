// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

internal class EChartLineOptionSerie
{
    public string Name { get; set; }

    public string Type { get; set; }

    public string Stack { get; set; }

    public string Symbol { get; set; }

    public int SymbolSize { get; set; }

    public EChartOptionItemStyle ItemStyle { get; set; }

    public EChartOptionItemStyle LineStyle { get; set; }

    public IEnumerable<object> Data { get; set; }
}
