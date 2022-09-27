// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

public class InstrumentRepository : Repository<TscDbContext, Instrument, Guid>, IInstrumentRepository
{
    private readonly TscDbContext _context;

    public InstrumentRepository(TscDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
        _context = context;
    }

    public async Task<Instrument> GetAsync(Guid Id, Guid userId)
    {
        return await _context.Set<Instrument>().Where(item => item.Id == Id && (item.IsGlobal || item.Creator == userId)).Include(d => d.Panels).ThenInclude(d => d.Metrics).FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("no data");
    }
}
