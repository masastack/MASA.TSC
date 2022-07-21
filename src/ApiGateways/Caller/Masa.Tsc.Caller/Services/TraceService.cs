// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Services;

public class TraceService : BaseService
{
    public TraceService(ICallerProvider caller) : base(caller, "/api/trace") { }

    public async Task<IEnumerable<object>> GetAsync(string traceId)
    {
        return (await Caller.GetAsync<IEnumerable<object>>($"{RootPath}/{traceId}"))!;
    }

    public async Task<PaginationDto<object>> GetListAsync(RequestTraceListDto model)
    {
        return (await Caller.GetAsync<PaginationDto<object>>($"{RootPath}/list", model))!;
    }

    public async Task<IEnumerable<string>> GetAttrValuesAsync(RequestAttrDataDto model)
    {
        return (await Caller.GetByBodyAsync<IEnumerable<string>>($"{RootPath}/attr-values", model))!;
    }

    public async Task<ChartLineDataDto<ChartPointDto>> GetAggegationAsync(RequestAggregationDto model)
    {
        return (await Caller.GetByBodyAsync<ChartLineDataDto<ChartPointDto>>($"{RootPath}/aggregation", model))!;
    }
}
