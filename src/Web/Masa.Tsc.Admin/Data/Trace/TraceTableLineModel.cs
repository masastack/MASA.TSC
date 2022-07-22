// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Data.Trace;

public class TraceTableLineModel
{
    public string Id { get; set; }

    public string ParentId { get; set; }

    public int Deep { get; set; }

    public bool IsTransaction { get; set; }

    public string ServiceName { get; set; }

    public DateTime Time { get; set; }

    public long Ms { get; set; }

    public string Left { get; set; }

    public string Width { get; set; }

    public string Right { get; set; }
}
