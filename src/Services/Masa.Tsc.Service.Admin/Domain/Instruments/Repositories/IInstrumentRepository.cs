// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Repositories;

internal interface IInstrumentRepository : IRepository<Instrument>
{
    Task<Instrument> GetAsync(Guid id, Guid userId);

    Task<List<Instrument>> GetListAsync(IEnumerable<Guid> ids, Guid userId);

    Task<Instrument> GetDetailAsync(Guid Id, Guid userId);

    Task<Instrument> UpdateDetailAsync(Instrument instrument);
}