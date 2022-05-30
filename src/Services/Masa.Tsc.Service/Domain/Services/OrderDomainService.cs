// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Domain.Services
{
    public class OrderDomainService : DomainService
    {
        private readonly ILogger<Order> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderDomainService(IDomainEventBus eventBus, ILogger<Order> logger, IOrderRepository orderRepository) : base(eventBus)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task PlaceOrderAsync()
        {
            //todo create order
            var orderEvent = new OrderCreatedDomainEvent();
            await EventBus.PublishAsync(orderEvent);
        }

        public async Task<IList<Order>> QueryListAsync()
        {
            return await _orderRepository.GetListAsync();
        }
    }
}