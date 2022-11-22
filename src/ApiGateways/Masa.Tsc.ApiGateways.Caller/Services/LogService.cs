// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class LogService : BaseService
{
    public LogService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/log",tokenProvider) { }   

    public async Task<TResult> AggregateAsync<TResult>(SimpleAggregateRequestDto model)
    {
        var str = await Caller.GetByBodyAsync<string>($"{RootPath}/aggregate", model);
        if (string.IsNullOrEmpty(str))
            return default!;
        return JsonSerializer.Deserialize<TResult>(str)!;
    }

    public async Task<LogResponseDto> GetLatestAsync(RequestLogLatestDto param)
    {
        return await Caller.GetByBodyAsync<LogResponseDto>($"{RootPath}/latest", param) ?? default!;
    }

    public async Task<IEnumerable<MappingResponseDto>> GetMappingFieldAsync()
    {
        return await Caller.GetAsync<IEnumerable<MappingResponseDto>>($"{RootPath}/mapping") ?? default!;
    }

    public async Task<PaginatedListBase<LogResponseDto>> GetPageAsync(LogPageQueryDto param)
    {
        return await Caller.GetAsync<PaginatedListBase<LogResponseDto>>($"{RootPath}/list", param) ?? default!;
    }
}