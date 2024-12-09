// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

internal class DirectoryRepository : Repository<TscDbContext, Domain.Shared.Entities.Directory, Guid>, IDirectoryRepository
{
    private readonly TscDbContext _context;

    public DirectoryRepository(TscDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
        _context = context;
    }

    public async Task<Domain.Shared.Entities.Directory> GetIncludeInstrumentsAsync(Guid id)
    {
        return (await _context.Set<Domain.Shared.Entities.Directory>().Where(item => item.Id == id).Include(d => d.Instruments!.OrderBy(d => d.Sort)).FirstOrDefaultAsync())!;
    }

    public async Task<Tuple<int, List<Domain.Shared.Entities.Directory>>> GetListIncludeInstrumentsAsync(Guid userId, int page, int pageSize, string keyword, bool isIncludeInstrument)
    {
        var start = page <= 1 ? 0 : (page - 1) * pageSize;
        var query = _context.Set<Domain.Shared.Entities.Directory>().AsQueryable();

        List<Domain.Shared.Entities.Directory> data;

        if (isIncludeInstrument)
            data = await query.Include(d => d.Instruments!.Where(instrument => string.IsNullOrEmpty(keyword) || instrument.Name.Contains(keyword)).OrderBy(d => d.Sort)).ToListAsync();
        else
            data = await query.ToListAsync();

        if (!string.IsNullOrEmpty(keyword))
            data = data.Where(item => item.Instruments != null && item.Instruments.Any()).ToList();

        var total = data.Count;
        data = data.Skip(start).Take(pageSize).ToList();

        var t = Tuple.Create(total, data);
        return t;
    }
}