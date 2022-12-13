// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Models;

public partial class LinkTrackingTopology : BDomComponentBase
{
    [Parameter]
    public LinkTrackingTopologyViewModel Value { get; set; } = new();

    private IJSObjectReference _helper = default!;
    private bool _helperRendered = false;

    protected override async Task OnParametersSetAsync()
    {
        if (_helperRendered)
        {
            await _helper.InvokeVoidAsync("update", Ref, Value);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        _helper = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/antv-g6/linkTrackingTopology-helper.js");
        _helperRendered = true;

        await RegisterEdge();
        await RegisterNode();
        await InitGraph();
    }

    public async Task RegisterEdge()
    {
        await _helper.InvokeVoidAsync("registerEdge");
    }

    public async Task RegisterNode()
    {
        await Task.CompletedTask;
    }

    public async Task InitGraph()
    {
        await _helper.InvokeVoidAsync("init", Ref, Value);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _helper.InvokeVoidAsync("destroy", Ref);

            if (_helper != null)
            {
                await _helper.DisposeAsync();
            }
        }
        catch (Exception)
        {
        }
    }
}
