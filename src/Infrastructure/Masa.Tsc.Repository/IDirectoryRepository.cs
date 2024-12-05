// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Repository;

public interface IDirectoryRepository : IRepository<Tsc.Domain.Instruments.Aggregates.Directory>
{
    Task<Tuple<int, List<Tsc.Domain.Instruments.Aggregates.Directory>>> GetListIncludeInstrumentsAsync(Guid userId, int page, int pageSize, string keyword, bool isIncludeInstrument);

    Task<Tsc.Domain.Instruments.Aggregates.Directory> GetIncludeInstrumentsAsync(Guid id);
}
