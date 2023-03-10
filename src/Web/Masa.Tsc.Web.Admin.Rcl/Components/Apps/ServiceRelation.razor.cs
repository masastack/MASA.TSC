// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class ServiceRelation
{
    [Parameter]
    public ModelTypes ModelType { get; set; }

    [Parameter]
    public string Service { get; set; }

    [Parameter]
    public EventCallback<string> ServiceChanged { get; set; }

    [Parameter]
    public string Relation { get; set; }

    [Parameter]
    public EventCallback<string> RelationChanged { get; set; }
}
