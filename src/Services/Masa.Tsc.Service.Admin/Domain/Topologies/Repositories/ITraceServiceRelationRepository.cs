// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Repositories;

public interface ITraceServiceRelationRepository
{
    public Task AddAsync(params TraceServiceRelation[] data);

    public Task<IEnumerable<TraceServiceRelation>> GetAllAsync();
}