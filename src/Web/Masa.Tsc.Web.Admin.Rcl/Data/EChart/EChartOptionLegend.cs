// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartOptionLegend
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EchartOrientTypes? Orient { get; set; }

    public bool? Left { get; set; }

    public bool? Right { get; set; }

    public bool? Top { get; set; }

    public bool? Bottom { get; set; }

    public IEnumerable<string> Data { get; set; }
}
