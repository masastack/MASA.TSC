// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;

public class TscGridstackJSModule : IAsyncDisposable
{
    readonly DotNetObjectReference<TscGridstackJSModule> _dotNetObjectReference;
    IJSRuntime _jsRuntime;
    IJSObjectReference? _helper;
    GridstackOptions? _options;
    public event Func<GridstackChangeEventArgs, Task>? OnChangeEvent;

    public TscGridstackJSModule(IJSRuntime js)
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        _jsRuntime = js;
    }

    async ValueTask<IJSObjectReference> GetJSObjectReference()
    {
        if (_helper is null) _helper = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/gridstack-helper.js");

        return _helper;
    }

    public async ValueTask InitAsync(GridstackOptions options)
    {
        _options = options;
        var helper = await GetJSObjectReference();
        await helper.InvokeAsync<IJSObjectReference>("init", _options, _dotNetObjectReference);
    }

    public async ValueTask InitAllAsync(GridstackOptions options)
    {
        _options = options;
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("initAll", _options, _dotNetObjectReference);
    }

    public async ValueTask ReloadAsync()
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("reload", _options, _dotNetObjectReference);
    }

    public async ValueTask MakeWidgetsAsync(IEnumerable<string> elementIds)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("makeWidgets", _options, elementIds);
    }

    public async ValueTask DestroyAsync(bool destoryDom)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("destroy", _options, destoryDom);
    }

    public async ValueTask CompactAsync()
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("compact", _options);
    }

    public async ValueTask SwitchStateAsync(bool enabled)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("switchState", _options, enabled);
    }

    [JSInvokable]
    public async Task OnChange(GridstackChangeEventArgs args)
    {
        if (OnChangeEvent is not null)
            await OnChangeEvent.Invoke(args);
    }

    public async ValueTask DisposeAsync()
    {
        OnChangeEvent = null;
        _dotNetObjectReference.Dispose();
        if (_helper is not null)
        {
            await _helper.DisposeAsync();
        }
    }
}
