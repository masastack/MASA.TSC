// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Domain.Shared.Entities;

namespace Masa.Tsc.Repository.Repositories;

internal class ExceptErrorRepository : Repository<TscDbContext, ExceptError>, IExceptErrorRepository
{
    private readonly DbSet<ExceptError> _exceptErrors;

    public ExceptErrorRepository(TscDbContext dbContext, IUnitOfWork unitOfWork, MasaStackClickhouseConnection clickhouseConnection, ILogger<ExceptErrorRepository> logger) : base(dbContext, unitOfWork)
    {
        _exceptErrors = dbContext.Set<ExceptError>();
    }

    public IEnumerable<ExceptError> ExceptErrors => _exceptErrors;

    public override async ValueTask<ExceptError> AddAsync(ExceptError entity, CancellationToken cancellationToken = default)
    {
        var result = await base.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override async Task<ExceptError> UpdateAsync(ExceptError entity, CancellationToken cancellationToken = default)
    {
        var result = await base.UpdateAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override async Task<ExceptError> RemoveAsync(ExceptError entity, CancellationToken cancellationToken = default)
    {
        var result = await base.RemoveAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override async Task RemoveRangeAsync(IEnumerable<ExceptError> entities, CancellationToken cancellationToken = default)
    {
        await base.RemoveRangeAsync(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}