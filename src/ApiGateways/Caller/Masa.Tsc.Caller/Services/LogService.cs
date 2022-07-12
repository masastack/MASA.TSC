// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Services;

public class LogService : BaseService
{
    public LogService(ICallerProvider caller) : base(caller, "/api/log") { }

    public async Task<IEnumerable<KeyValuePair<string, string>>> AggegationAsync(RequestLogAggDto param)
    {
        return await Caller.GetByBodyAsync<IEnumerable<KeyValuePair<string, string>>>($"{RootPath}/aggregation", param) ?? default!;
    }

    public async Task<object> GetLatestAsync(RequestLogLatestDto param)
    {
        return await Caller.GetByBodyAsync<object>($"{RootPath}/latest", param) ?? default!;
    }

    public async Task<IEnumerable<Nest.MappingResponse>> GetMappingFieldAsync()
    {
        return await Caller.GetAsync<IEnumerable<Nest.MappingResponse>>($"{RootPath}/mapping") ?? default!;
    }

    public async Task<PaginationDto<object>> GetPageAsync(LogPageQueryDto param)
    {
        return await Caller.GetAsync<PaginationDto<object>>($"{RootPath}/list",param) ?? default!;
    }
}
