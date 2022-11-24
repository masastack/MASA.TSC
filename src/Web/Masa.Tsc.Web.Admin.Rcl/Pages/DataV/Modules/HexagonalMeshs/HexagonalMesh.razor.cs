// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.JSInterop;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class HexagonalMesh : BDomComponentBase
{
    [Parameter]
    public List<HexagonalMeshViewModel> Value { get; set; } = default!;

    private IJSObjectReference _helper = default!;
    private bool _helperRendered = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        _helper = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/antv-g2/hexagonalMesh-helper.js");
        _helperRendered = true;

        await InitChart();
        await AddPolygon();
        await Render();
    }

    public async Task InitChart()
    {
        await _helper.InvokeVoidAsync("init", Ref, Value);
    }

    public async Task AddPolygon()
    {
        await _helper.InvokeVoidAsync("addPolygon", Ref, Value);
    }

    public async Task Render()
    {
        await _helper.InvokeVoidAsync("render", Ref);
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
