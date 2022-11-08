// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.HexagonalMeshs;

public class HexagonalMeshViewModel
{
    public string Key { get; set; } = default!;

    public int Q { get; set; }

    public int R { get; set; }

    public string Name { get; set; } = default!;

    public LinkTrackingTopologyStatuses State { get; set; }

    public List<HexagonalMeshItemViewModel> Items { get; set; } = new();
}
