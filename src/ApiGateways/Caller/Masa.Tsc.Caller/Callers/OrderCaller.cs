// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Order.Model;

namespace Masa.Tsc.Caller.Callers
{
    public class OrderCaller : HttpClientCallerBase
    {
        protected override string BaseAddress { get; set; } = "http://localhost:6024";

        public OrderCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = nameof(OrderCaller);
        }

        public async Task<List<Order>> GetListAsync()
        {
            return await CallerProvider.GetAsync<List<Order>>($"order/list");
        }
    }
}