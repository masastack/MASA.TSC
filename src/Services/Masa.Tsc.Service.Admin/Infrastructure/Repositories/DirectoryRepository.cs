// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

public class DirectoryRepository : Repository<TscDbContext, Domain.Aggregates.Directory, Guid>, IDirectoryRepository
{
    private readonly TscDbContext _context;

    public DirectoryRepository(TscDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
        _context = context;
    }

    public async Task<Domain.Aggregates.Directory> GetIncludeInstrumentsAsync(Guid id)
    {
        return (await _context.Set<Domain.Aggregates.Directory>().Where(item => item.Id == id).Include(d => d.Instruments.OrderBy(d => d.Sort)).FirstOrDefaultAsync())!;
    }

    public async Task<Tuple<int, List<Domain.Aggregates.Directory>>> GetListIncludeInstrumentsAsync(Guid userId, int page, int pageSize, string keyword, bool isIncludeInstrument)
    {
        var start = page <= 1 ? 0 : (page - 1) * pageSize;
        var dbSet = _context.Set<Domain.Aggregates.Directory>();
        var query = dbSet.Skip(start).Take(pageSize);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(m => m.Name.Contains(keyword));
        var total = await query.CountAsync();
        if (isIncludeInstrument)
            query = query.Include(d => d.Instruments.OrderBy(d => d.Sort));

        return Tuple.Create(total, await query.ToListAsync());
    }
}