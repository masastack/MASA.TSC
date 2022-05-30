// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Application.Orders
{
    public class OrderQueryHandler
    {
        readonly IOrderRepository _orderRepository;
        public OrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [EventHandler]
        public async Task OrderListHandleAsync(OrderQuery query)
        {
            query.Result = await _orderRepository.GetListAsync();
        }
    }
}