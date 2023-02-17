// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class TraceService : BaseService
{
    public TraceService(ICaller caller) : base(caller, "/api/trace") { }

    public async Task<IEnumerable<TraceResponseDto>> GetAsync(string traceId)
    {
        return await Caller.GetAsync<IEnumerable<TraceResponseDto>>($"{RootPath}/{traceId}") ?? Array.Empty<TraceResponseDto>();
    }

    public async Task<PaginatedListBase<TraceResponseDto>> GetListAsync(RequestTraceListDto model, CancellationToken? token = null)
    {
        return await Caller.GetAsync<PaginatedListBase<TraceResponseDto>>($"{RootPath}/list", model, token ?? default) ?? new PaginatedListBase<TraceResponseDto>();
    }

    public async Task<IEnumerable<string>> GetAttrValuesAsync(SimpleAggregateRequestDto model)
    {
        return await Caller.GetByBodyAsync<IEnumerable<string>>($"{RootPath}/attr-values", model) ?? Array.Empty<string>();
    }

    public async Task<TResult> AggregateAsync<TResult>(SimpleAggregateRequestDto model)
    {
        var str = await Caller.GetByBodyAsync<string>($"{RootPath}/aggregate", model);
        if (string.IsNullOrEmpty(str))
            return default!;
        return JsonSerializer.Deserialize<TResult>(str)!;
    }
}
