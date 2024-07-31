// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse;

internal class LogService : ILogService
{
    private readonly IDbConnection _dbConnection;
    public LogService(MasaStackClickhouseConnection connection)
    {
        _dbConnection = connection;
    }

    public Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return Task.FromResult(_dbConnection.AggregationQuery(query));
    }

    public Task<PaginatedListBase<LogResponseDto>> ListAsync(BaseRequestDto query)
    {
        return Task.FromResult(_dbConnection.QueryLog(query));
    }

    public Task<IEnumerable<MappingResponseDto>> GetMappingAsync()
    {
        return Task.FromResult(_dbConnection.GetMapping(true).AsEnumerable());
    }
}
