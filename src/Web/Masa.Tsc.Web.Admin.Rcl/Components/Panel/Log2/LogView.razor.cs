// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log2;

public partial class LogView
{
    [Parameter]
    public Dictionary<string, LogTree>? JsonObject { get; set; }

    [Parameter]
    public List<LogTree>? JsonArray { get; set; }
}
