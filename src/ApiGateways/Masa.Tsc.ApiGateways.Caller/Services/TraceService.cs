// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class TraceService : BaseService
{
    public TraceService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/trace",tokenProvider) { }

    public async Task<IEnumerable<TraceDto>> GetAsync(string traceId)
    {
        return (await Caller.GetAsync<IEnumerable<TraceDto>>($"{RootPath}/{traceId}"))!;
    }

    public async Task<PaginationDto<TraceDto>> GetListAsync(RequestTraceListDto model)
    {
        return (await Caller.GetAsync<PaginationDto<TraceDto>>($"{RootPath}/list", model))!;
    }

    public async Task<IEnumerable<string>> GetAttrValuesAsync(RequestAttrDataDto model)
    {
        return (await Caller.GetByBodyAsync<IEnumerable<string>>($"{RootPath}/attr-values", model))!;
    }

    public async Task<ChartLineDataDto<ChartPointDto>> AggregateAsync(RequestAggregationDto model)
    {
        return (await Caller.GetByBodyAsync<ChartLineDataDto<ChartPointDto>>($"{RootPath}/aggregate", model))!;
    }
}
