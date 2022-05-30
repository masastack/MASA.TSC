// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Order.Model
{
    public class Order
    {
        public DateTime CreationTime { get; set; }

        public int Id { get; set; }

        public string OrderNumber { get; set; } = "";

        public string Address { get; set; } = default!;
    }
}