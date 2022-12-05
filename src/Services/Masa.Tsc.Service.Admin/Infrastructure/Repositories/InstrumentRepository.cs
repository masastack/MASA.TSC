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

    public async Task<Instrument> GetAsync(Guid id, Guid userId)
    {
        return (await _context.Set<Instrument>().FirstOrDefaultAsync(item => item.Id == id && (item.IsGlobal || item.Creator == userId)))!;
    }

    public async Task<List<Instrument>> GetListAsync(IEnumerable<Guid> ids, Guid userId)
    {
        return await _context.Set<Instrument>().Where(item => ids.Contains(item.Id) && (item.IsGlobal || item.Creator == userId)).ToListAsync();
    }

    public async Task<Instrument> GetDetailAsync(Guid Id, Guid userId)
    {
        return await _context.Set<Instrument>().Where(item => item.Id == Id && (item.IsGlobal || item.Creator == userId))
            .Include(d => d.Panels.OrderBy(d => d.Index))
            .ThenInclude(d => d.Metrics.OrderBy(d => d.Sort)).FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("no data");
    }

    public async Task<Instrument> GetIncludePanelsAsync(Guid Id, Guid userId)
    {
        return await _context.Set<Instrument>().Where(item => item.Id == Id && (item.IsGlobal || item.Creator == userId))
            .Include(d => d.Panels.OrderBy(d => d.Index)).FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("no data");
    }
}
