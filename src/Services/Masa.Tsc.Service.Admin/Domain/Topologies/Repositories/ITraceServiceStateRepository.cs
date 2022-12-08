// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Topologies.Repositories;

public interface ITraceServiceStateRepository
{
    public Task AddAsync(params TraceServiceState[] data);

    public Task<List<TopologyServiceDataDto>> GetServiceTermsDataAsync(DateTime start, DateTime end, params string[] services);
}