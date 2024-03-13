// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class ApmService : ServiceBase
{
    private readonly IApmService apmService;
    public ApmService(IApmService apmService) : base("/api/apm")
    {
        this.apmService = apmService;
    }

    public async Task<PaginatedListBase<ServiceListDto>> GetServices(int page, int pageSize, DateTime start, DateTime end, string? env, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.ServicePageAsync(new BaseApmRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<PaginatedListBase<EndpointListDto>> GetEndpoints(int page, int pageSize, DateTime start, DateTime end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.EndpointPageAsync(new BaseApmRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            Service = service,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<IEnumerable<ChartLineDto>> GetCharts(DateTime start, DateTime end, string? env, string? service, ComparisonTypes? comparisonType, string? queries)
        => await apmService.ChartDataAsync(new BaseApmRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            Service = service,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<EndpointLatencyDistributionDto> GetLatencyDistributions(DateTime start, DateTime end, string? env, string? service, string endpoint)
        => await apmService.EndpointLatencyDistributionAsync(new ApmEndpointRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            Service = service,
            Endpoint = endpoint,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<PaginatedListBase<ErrorMessageDto>> GetErrors(int page, int pageSize, DateTime start, DateTime end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.ErrorMessagePageAsync(new ApmEndpointRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            Service = service
        });

    public Task<IEnumerable<ChartPointDto>> GetSpanErrors(int page, int pageSize, DateTime start, DateTime end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => apmService.GetTraceErrorsAsync(new ApmEndpointRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            Service = service
        });

    public async Task<PaginatedListBase<TraceResponseDto>> GetTraceDetail(int page, DateTime start, DateTime end, string? env, string? service, string endpoint, long? latMin, long? latMax)
        => await apmService.TraceLatencyDetailAsync(new ApmTraceLatencyRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            Page = page,
            Service = service,
            Endpoint = endpoint,
            LatMax = latMax,
            LatMin = latMin
        });

    public async Task<IEnumerable<ChartLineCountDto>> GetErrorChart(DateTime start, DateTime end, string? env, string? service, string? endpoint, ComparisonTypes? comparisonType, string? queries)
         => await apmService.GetErrorChartAsync(new ApmEndpointRequestDto
         {
             Start = start,
             End = end,
             Env = GetEnv(env),
             ComparisonType = comparisonType,
             Queries = queries,
             Service = service,
             Endpoint = endpoint!,
             StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
         });

    public async Task<IEnumerable<ChartLineCountDto>> GetLogChart(DateTime start, DateTime end, string? env, string? service, ComparisonTypes? comparisonType, string? queries)
        => await apmService.GetLogChartAsync(new ApmEndpointRequestDto
        {
            Start = start,
            End = end,
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            Service = service
        });

    private static string? GetEnv(string? env) => string.Equals("all", env, StringComparison.CurrentCultureIgnoreCase) ? default : env;
}