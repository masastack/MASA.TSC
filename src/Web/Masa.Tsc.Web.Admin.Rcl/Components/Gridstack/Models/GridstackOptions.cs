// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;

public class GridstackOptions
{
    public string Id { get; set; } = "grid-stack";

    public int Column { get; set; } = 12;

    public bool AcceptWidgets { get; set; }

    public bool AlwaysShowResizeHandle { get; set; } = true;

    public bool Animate { get; set; } = true;

    public string CellHeight { get; set; } = "auto";

    public int CellHeightThrottle { get; set; } = 100;

    public int MinRow { get; set; } = 1;

    public string? Nonce { get; set; }

    public int Margin { get; set; }

    public int MarginTop { get; set; } = 10;

    public int MarginRight { get; set; } = 10;

    public int MarginBottom { get; set; }

    public int MarginLeft { get; set; }

    public bool Float { get; set; }

    public bool DisableResize { get; set; }

    public bool DisableDrag { get; set; }

    public bool OneColumnModeDomSort { get; set; }

    public bool DisableOneColumnMode { get; set; } = true;

    public string Handle { get; set; } = ".draggable";
}
