// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Contracts;

public interface ITraceService
{
    Task<IEnumerable<TraceResponseDto>> GetAsync(BaseRequestDto query);

    Task<PaginatedListBase<TraceResponseDto>> ListAsync(BaseRequestDto query);

    Task<PaginatedListBase<TraceResponseDto>> ScrollAsync(BaseRequestDto query);

    /// <summary>
    /// when query type: Count,Sum,Avg and DistinctCount return type is double, DateHistogram return IEnumerable<KeyValuePair<double, long>> ,GroupBy return IEnumerable<string>
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<object> AggregateAsync(SimpleAggregateRequestDto query);

    Task<string> GetMaxDelayTraceIdAsync(BaseRequestDto query);
}
