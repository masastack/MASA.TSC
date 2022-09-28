﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Repositories;

public interface IInstrumentRepository : IRepository<Instrument>
{
    Task<Instrument> GetAsync(Guid Id,Guid userId);
}
