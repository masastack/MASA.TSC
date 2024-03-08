﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public sealed class LogService : BaseService
{
    internal LogService(ICaller caller) : base(caller, "/api/log") { }

    public async Task<TResult> AggregateAsync<TResult>(SimpleAggregateRequestDto model)
    {
        var str = await Caller.GetByBodyAsync<string>($"{RootPath}/aggregate", model);
        if (string.IsNullOrEmpty(str))
            return default!;
        return JsonSerializer.Deserialize<TResult>(str, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;
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

    public async Task<PaginatedListBase<LogDto>> GetDynamicPageAsync(LogPageQueryDto param)
    {
        return await Caller.GetAsync<PaginatedListBase<LogDto>>($"{RootPath}/list", param) ?? default!;
    }

    public async Task<List<LogErrorDto>> GetErrorTypesAsync(string service,DateTime start,DateTime end)
    {
        return await Caller.GetAsync<List<LogErrorDto>>($"{RootPath}/errorTypes", new {service,start,end }) ?? default!;
    }
}