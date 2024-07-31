// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Repositories;

public interface IDirectoryRepository : IRepository<Aggregates.Directory>
{
    Task<Tuple<int, List<Aggregates.Directory>>> GetListIncludeInstrumentsAsync(Guid userId, int page, int pageSize, string keyword, bool isIncludeInstrument);

    Task<Aggregates.Directory> GetIncludeInstrumentsAsync(Guid id);
}
