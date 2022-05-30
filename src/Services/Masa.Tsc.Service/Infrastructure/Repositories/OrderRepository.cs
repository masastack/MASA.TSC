// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Infrastructure.Repositories
{
    public class OrderRepository : Repository<ShopDbContext, Order>, IOrderRepository
    {
        public OrderRepository(ShopDbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }
        public async Task<List<Order>> GetListAsync()
        {
            var data = Enumerable.Range(1, 5).Select(index =>
                  new Order
                  {
                      CreationTime = DateTimeOffset.Now,
                      Id = index,
                      OrderNumber = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                      Address = $"Address {index}"
                  }).ToList();
            return await Task.FromResult(data);
        }
    }
}