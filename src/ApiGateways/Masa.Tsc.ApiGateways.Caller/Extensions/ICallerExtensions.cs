// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal static class ICallerExtensions
{
    public static async Task<TResult> GetByBodyAsync<TResult>(this ICaller caller, string url, object body) where TResult : class
    {

        if (caller is DaprCaller daprCaller)
        {
            var request = daprCaller.CreateRequest(HttpMethod.Get, url, body);
            return (await daprCaller.SendAsync<TResult>(request, default)) ?? default!;
        }
        else
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (body != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }
            return (await caller.SendAsync<TResult>(request, default)) ?? default!;
        }
    }
}
