// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class HexagonalMeshViewModel
{
    public string Key { get; set; } = default!;

    /// <summary>
    /// column num ,start width 0,the left is 0
    /// </summary>
    public int Q { get; set; }

    /// <summary>
    /// row position,start with -1,the first row number is largest,the last row number is -1
    /// </summary>
    public int R { get; set; }

    public string Name { get; set; } = default!;

    public MonitorStatuses State { get; set; }

    public List<AppDto> Items { get; set; } = new();
}
