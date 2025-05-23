// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse;

internal class TraceService : ITraceService
{
    private readonly IDbConnection _dbConnection;

    public TraceService(MasaStackClickhouseConnection connection)
    {
        _dbConnection = connection;
    }

    public Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return Task.FromResult(_dbConnection.AggregationQuery(query, false));
    }

    public Task<IEnumerable<TraceResponseDto>> GetAsync(BaseRequestDto query)
    {
        return Task.FromResult(_dbConnection.GetTraceByTraceId(query).AsEnumerable());
    }

    public Task<string> GetMaxDelayTraceIdAsync(BaseRequestDto query)
    {
        return Task.FromResult(_dbConnection.GetMaxDelayTraceId(query));
    }

    public Task<PaginatedListBase<TraceResponseDto>> ListAsync(BaseRequestDto query)
    {
        return Task.FromResult(_dbConnection.QueryTrace(query));
    }

    public Task<PaginatedListBase<TraceResponseDto>> ScrollAsync(BaseRequestDto query)
    {
        return Task.FromResult(_dbConnection.QueryTrace(query));
    }
}
