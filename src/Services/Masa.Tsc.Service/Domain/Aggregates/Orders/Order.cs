// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Domain.Aggregates.Orders
{
    public class Order : AggregateRoot<int>
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

        public string OrderNumber { get; set; } = default!;

        public string Address { get; set; } = default!;

        public List<OrderItem> Items { get; set; }
    }
}