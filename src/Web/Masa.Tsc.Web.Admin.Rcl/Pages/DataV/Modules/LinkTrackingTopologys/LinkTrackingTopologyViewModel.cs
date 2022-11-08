// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys;

public class LinkTrackingTopologyViewModel
{
    public List<LinkTrackingTopologyNodeViewModel> Nodes { get; set; } = new();

    public List<LinkTrackingTopologyEdgeViewModel> Edges { get; set; } = new();
}
