// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Repository;

public interface IDirectoryRepository : IRepository<Domain.Shared.Entities.Directory>
{
    Task<Tuple<int, List<Domain.Shared.Entities.Directory>>> GetListIncludeInstrumentsAsync(Guid userId, int page, int pageSize, string keyword, bool isIncludeInstrument);

    Task<Domain.Shared.Entities.Directory> GetIncludeInstrumentsAsync(Guid id);
}
