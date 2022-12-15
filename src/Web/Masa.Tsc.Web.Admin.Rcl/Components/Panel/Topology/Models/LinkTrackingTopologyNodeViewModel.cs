// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Topology.Models;

public class LinkTrackingTopologyNodeViewModel
{
    public string Id { get; set; }

    public string Label { get; set; }

    public int Depth { get; set; }

    public MonitorStatuses State { get; set; }

    public int X { get; set; }

    public int Y { get; set; }
}
