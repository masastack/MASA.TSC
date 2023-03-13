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
    public string Instance { get; set; }

    [Parameter]
    public string Endpoint { get; set; }

    [Parameter]
    public EventCallback<(string?, string?, string?)> ValueChanged { get; set; }

    async Task ServiceChanged(string service)
    {
        Service = service;
        if (ModelType is ModelTypes.Service)
        {
            await ValueChanged.InvokeAsync((Service, default, default));
        }
    }

    async Task InstanceChanged(string instance)
    {
        Instance = instance;
        if (ModelType is ModelTypes.ServiceInstance)
        {
            await ValueChanged.InvokeAsync((Service, Instance, default));
        }
    }

    async Task EndpointChanged(string endpoint)
    {
        Endpoint = endpoint;
        if (ModelType is ModelTypes.Endpoint)
        {
            await ValueChanged.InvokeAsync((Service, Instance, Endpoint));
        }
    }
}
