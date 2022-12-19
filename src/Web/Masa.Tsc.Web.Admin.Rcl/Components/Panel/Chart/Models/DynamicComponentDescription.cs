// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public class DynamicComponentDescription
{
    public Type ShowType { get; set; }

    public Dictionary<string, object?> Metadata { get; set; }

    public DynamicComponentDescription(Type showType, Dictionary<string, object?> metadata)
    {
        ShowType = showType;
        Metadata = metadata;
    }
}
