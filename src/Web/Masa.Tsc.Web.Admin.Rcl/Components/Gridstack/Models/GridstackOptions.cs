// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;

public class GridstackOptions
{
    public string Id { get; set; } = "grid-stack";

    public int Column { get; set; } = 12;

    public bool AcceptWidgets { get; set; } = true;

    public bool AlwaysShowResizeHandle { get; set; } = true;

    public bool Animate { get; set; } = true;

    public string CellHeight { get; set; } = "auto";

    public int CellHeightThrottle { get; set; } = 100;

    public string? Nonce { get; set; }

    public int Margin { get; set; } = 10;

    public bool Float { get; set; }

    public bool DisableResize { get; set; }

    public bool DisableDrag { get; set; }

    public bool OneColumnModeDomSort { get; set; }

    public bool DisableOneColumnMode { get; set; } = true;

    [JsonIgnore]
    public Func<IEnumerable<GridstackChangeEventArgs>, Task>? OnChange { get; set; }
}
