// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Repositories;

public interface ITraceServiceNodeRepository
{
    public Task AddAsync(params TraceServiceNode[] data);

    public Task<IEnumerable<TraceServiceNode>> GetAllAsync();
}