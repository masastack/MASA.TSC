// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Application.Orders.Commands
{
    public record OrderCreateCommand : DomainCommand
    {
        public List<OrderItem> Items { get; set; } = new();
    }
}