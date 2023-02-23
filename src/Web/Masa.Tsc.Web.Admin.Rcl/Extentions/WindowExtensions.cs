// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public static class WindowExtensions
{
    public static async Task AddEventListenerAsync(this Window window, string type, string selector, Func<Task> listener, OneOf<EventListenerOptions, bool> options)
    {
        await window.JS.InvokeVoidAsync(
            JsInteropConstants.AddHtmlElementEventListener,
            selector,
            type,
            DotNetObjectReference.Create(new Invoker<object>(_ => { listener?.Invoke(); })),
            options.Value);
    }
}
