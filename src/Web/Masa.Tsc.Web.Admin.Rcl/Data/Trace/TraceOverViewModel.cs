// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Trace;

public class TraceOverviewModel : TraceTimeUsModel
{
    public TraceOverviewModel() : base(1) { }

    public string Name { get; set; }

    public int Total { get; set; }

    public DateTime Start { get; set; }

    public List<TraceOverviewServiceModel> Services { get; set; } = new List<TraceOverviewServiceModel>();

    public Dictionary<string, TraceTableLineModel> SpansDeeps = new();

    public Dictionary<string, List<string>> SpanChildren = new();

    public string SearchName { get; set; }
}

public class TraceOverviewServiceModel
{
    public string Name { get; set; }

    public string Color { get; set; }
}
