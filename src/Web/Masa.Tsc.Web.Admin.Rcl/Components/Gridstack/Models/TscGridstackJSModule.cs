// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;

public class TscGridstackJSModule : IAsyncDisposable
{
    readonly DotNetObjectReference<TscGridstackJSModule> _dotNetObjectReference;
    IJSRuntime _jsRuntime;
    IJSObjectReference? _helper;
    GridstackOptions? _options;
    public event Func<IEnumerable<GridstackChangeEventArgs>, Task>? OnChangeEvent;
    public event Func<IEnumerable<GridstackAddEventArgs>, Task>? OnAddEvent;
    public event Func<IEnumerable<GridstackRemoveEventArgs>, Task>? OnRemoveEvent;

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
        await helper.InvokeVoidAsync("reload", _options);
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

    public async ValueTask RemoveAllAsync(bool destoryDom)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("removeAll", _options, destoryDom);
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

    public async ValueTask SaveAsync()
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("save", _options, _dotNetObjectReference);
    }

    public async ValueTask Movable(Guid itemId, bool enabled)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("movable", _options, itemId, enabled);
    }

    public async ValueTask Resizable(Guid itemId, bool enabled)
    {
        var helper = await GetJSObjectReference();
        await helper.InvokeVoidAsync("resizable", _options, itemId, enabled);
    }

    [JSInvokable]
    public async Task OnChange(IEnumerable<GridstackChangeEventArgs> args)
    {
        if (OnChangeEvent is not null)
            await OnChangeEvent.Invoke(args);
    }

    [JSInvokable]
    public async Task OnAdd(IEnumerable<GridstackAddEventArgs> args)
    {
        if (OnAddEvent is not null)
            await OnAddEvent.Invoke(args);
    }

    [JSInvokable]
    public async Task OnRemove(IEnumerable<GridstackRemoveEventArgs> args)
    {
        if (OnRemoveEvent is not null)
            await OnRemoveEvent.Invoke(args);
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
