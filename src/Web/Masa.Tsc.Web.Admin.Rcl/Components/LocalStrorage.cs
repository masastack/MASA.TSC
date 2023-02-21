// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components
{
    public class LocalStrorage
    {
        public LocalStrorage(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        IJSRuntime jSRuntime { get; set; }

        readonly JsonSerializerOptions options = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task SetItem(string key, object value)
        {
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, options));
        }

        public async Task<T> GetItem<T>(string key)
        {
            var value = await jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (value == null)
                return default!;
            var type = typeof(T);
            if (type == typeof(string))
                return (T)(object)value;
            return JsonSerializer.Deserialize<T>(value, options)!;
        }

        public async Task RemoveItem(string key)
        {
             await jSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task Clear()
        {
            await jSRuntime.InvokeVoidAsync("localStorage.clear");
        }
    }
}
