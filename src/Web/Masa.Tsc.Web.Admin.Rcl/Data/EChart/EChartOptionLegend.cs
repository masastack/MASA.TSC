// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

internal class EChartOptionLegend
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EchartOrientTypes? Orient { get; set; }

    public string Left { get; set; }

    public string Right { get; set; }

    public string Top { get; set; }

    public string Bottom { get; set; }

    public IEnumerable<string> Data { get; set; }
}
