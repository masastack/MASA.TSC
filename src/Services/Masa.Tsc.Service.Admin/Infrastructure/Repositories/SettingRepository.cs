// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

public class SettingRepository : Repository<TscDbContext, Setting, Guid>, ISettingRepository
{
    public SettingRepository(TscDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }    
}
