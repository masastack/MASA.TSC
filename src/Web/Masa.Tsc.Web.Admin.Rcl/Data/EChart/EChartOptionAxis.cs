﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class EChartOptionAxis
{
    public string Type { get; set; }

    public bool BoundaryGap { get; set; }

    public IEnumerable<string> Data { get; set; }

    public EChartOptionAxisLine AxisLine { get; set; }
}