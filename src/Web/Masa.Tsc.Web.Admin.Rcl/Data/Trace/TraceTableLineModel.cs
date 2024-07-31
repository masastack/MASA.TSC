// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Trace;

public class TraceTableLineModel : TraceTimeUsModel
{
    public TraceTableLineModel() : base(1) { }

    public string Id { get; set; }

    public string ParentId { get; set; }

    public int Deep { get; set; }

    public bool IsTransaction { get; set; }

    public string ServiceName { get; set; }

    public DateTime Time { get; set; }

    public string Left { get; set; }

    public string Width { get; set; }

    public string Right { get; set; }

    public string Color { get; set; }
}
