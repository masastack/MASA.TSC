// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Data.Trace;

public class TraceOverViewModel : TraceTimeUsModel
{
    public TraceOverViewModel() : base(1) { }

    public string Name { get; set; }

    public int Total { get; set; }

    public DateTime Start { get; set; }

    public List<TraceOverViewServiceModel> Services { get; set; } = new List<TraceOverViewServiceModel>();
}

public class TraceOverViewServiceModel
{
    public string Name { get; set; }

    public string Color { get; set; }
}
