// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Domain.Aggregates.Orders
{
    public class OrderItem : Entity<int>
    {
        public int ProductId { get; set; }

        public float Price { get; set; }
    }
}