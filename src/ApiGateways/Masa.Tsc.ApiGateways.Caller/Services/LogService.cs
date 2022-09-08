// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class LogService : BaseService
{
    public LogService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/log",tokenProvider) { }

    public async Task<IEnumerable<KeyValuePair<string, string>>> AggregateAsync(RequestAggregationDto param)
    {
        return await Caller.GetByBodyAsync<IEnumerable<KeyValuePair<string, string>>>($"{RootPath}/aggregate", param) ?? default!;
    }

    public async Task<object> GetLatestAsync(RequestLogLatestDto param)
    {
        return await Caller.GetByBodyAsync<object>($"{RootPath}/latest", param) ?? default!;
    }

    public async Task<IEnumerable<MappingResponse>> GetMappingFieldAsync()
    {
        return await Caller.GetAsync<IEnumerable<MappingResponse>>($"{RootPath}/mapping") ?? default!;
    }

    public async Task<PaginationDto<object>> GetPageAsync(LogPageQueryDto param)
    {
        return await Caller.GetAsync<PaginationDto<object>>($"{RootPath}/list", param) ?? default!;
    }
}
