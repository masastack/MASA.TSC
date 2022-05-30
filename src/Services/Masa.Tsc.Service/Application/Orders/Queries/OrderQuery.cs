// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Application.Orders.Queries
{
    public record OrderQuery : DomainQuery<List<Order>>
    {
        public override List<Order> Result { get; set; } = new();
    }
}