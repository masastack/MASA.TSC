// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public sealed class ApmService : BaseService
{
    internal ApmService(ICaller caller) : base(caller, "/api/apm") { }

    public Task<PaginatedListBase<ServiceListDto>> GetServicePageAsync(BaseApmRequestDto query) => Caller.GetAsync<PaginatedListBase<ServiceListDto>>($"{RootPath}/services", data: query)!;

    public Task<PaginatedListBase<EndpointListDto>> GetEndpointPageAsync(BaseApmRequestDto query) => Caller.GetAsync<PaginatedListBase<EndpointListDto>>($"{RootPath}/endpoints", data: query)!;

    public Task<PaginatedListBase<ErrorMessageDto>> GetErrorsPageAsync(ApmEndpointRequestDto query) => Caller.GetAsync<PaginatedListBase<ErrorMessageDto>>($"{RootPath}/errors", data: query)!;

    public Task<List<ChartPointDto>> GetSpanErrorsAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartPointDto>>($"{RootPath}/spanErrors", data: query)!;

    public Task<PaginatedListBase<TraceResponseDto>> GetTraceDetailAsync(ApmTraceLatencyRequestDto query) => Caller.GetAsync<PaginatedListBase<TraceResponseDto>>($"{RootPath}/traceDetail", data: query)!;

    public Task<List<ChartLineDto>> GetChartsAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineDto>>($"{RootPath}/charts", data: query)!;

    public Task<List<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineCountDto>>($"{RootPath}/errorChart", data: query)!;

    public Task<List<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineCountDto>>($"{RootPath}/logChart", data: query)!;

    public Task<EndpointLatencyDistributionDto> GetLatencyDistributionAsync(ApmEndpointRequestDto query) => Caller.GetAsync<EndpointLatencyDistributionDto>($"{RootPath}/latencyDistributions", data: query)!;
}