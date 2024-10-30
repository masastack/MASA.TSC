// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public sealed class ApmService : BaseService
{
    internal ApmService(ICaller caller) : base(caller, "/api/apm") { }

    public Task<PaginatedListBase<ServiceListDto>> GetServicePageAsync(Guid teamId, BaseApmRequestDto query, string? projectId = default, string? appType = default) => Caller.GetAsync<PaginatedListBase<ServiceListDto>>($"{RootPath}/services?teamId={teamId}&project={projectId}{(string.IsNullOrEmpty(appType) ? "" : $"&appType={appType}")}", data: query)!;

    public Task<PaginatedListBase<EndpointListDto>> GetEndpointPageAsync(Guid teamId, BaseApmRequestDto query, string? projectId = default, string? appType = default) => Caller.GetAsync<PaginatedListBase<EndpointListDto>>($"{RootPath}/endpoints?teamId={teamId}&project={projectId}{(string.IsNullOrEmpty(appType) ? "" : $"&appType={appType}")}", data: query)!;

    public Task<PaginatedListBase<ErrorMessageDto>> GetErrorsPageAsync(Guid teamId, ApmEndpointRequestDto query, string? projectId = default, string? appType = default, bool ignoreTeam = false) => Caller.GetAsync<PaginatedListBase<ErrorMessageDto>>($"{RootPath}/errors?teamId={teamId}&project={projectId}&ignoreTeam={ignoreTeam}{(string.IsNullOrEmpty(appType) ? "" : $"&appType={appType}")}", data: query)!;

    public Task<List<ChartPointDto>> GetSpanErrorsAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartPointDto>>($"{RootPath}/spanErrors", data: query)!;

    public Task<PaginatedListBase<TraceResponseDto>> GetTraceDetailAsync(ApmTraceLatencyRequestDto query) => Caller.GetAsync<PaginatedListBase<TraceResponseDto>>($"{RootPath}/traceDetail", data: query)!;

    public Task<List<ChartLineDto>> GetChartsAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineDto>>($"{RootPath}/charts", data: query)!;

    public Task<List<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineCountDto>>($"{RootPath}/errorChart", data: query)!;

    public Task<List<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query) => Caller.GetAsync<List<ChartLineCountDto>>($"{RootPath}/logChart", data: query)!;

    public Task<EndpointLatencyDistributionDto> GetLatencyDistributionAsync(ApmEndpointRequestDto query) => Caller.GetAsync<EndpointLatencyDistributionDto>($"{RootPath}/latencyDistributions", data: query)!;

    public Task<Dictionary<string, List<EnviromentAppDto>>> GetEnviromentServiceAsync(Guid teamId, DateTime start, DateTime end, string? env = default, bool ignoreTeam = false) => Caller.GetAsync<Dictionary<string, List<EnviromentAppDto>>>($"{RootPath}/enviromentService", data: new { teamId, start, end, env, ignoreTeam })!;

    public Task<PaginatedListBase<TraceResponseDto>> GetTraceListAsync(BaseRequestDto query) => Caller.GetByBodyAsync<PaginatedListBase<TraceResponseDto>>($"{RootPath}/traceList", body: query)!;

    public Task<PaginatedListBase<LogResponseDto>> GetLogListAsync(Guid teamId, BaseRequestDto query, string? projectId = default, string? appType = default, bool ignoreTeam = false) => Caller.GetByBodyAsync<PaginatedListBase<LogResponseDto>>($"{RootPath}/logList?teamId={teamId}&project={projectId}&&ignoreTeam={ignoreTeam}{(string.IsNullOrEmpty(appType) ? "" : $"&appType={appType}")}", body: query)!;

    public Task<PhoneModelDto> GetDeviceModelAsync(string brand, string model) => Caller.GetAsync<PhoneModelDto>($"{RootPath}/model?brand={brand}&model={model}")!;

    public Task<List<string>> GetStatusCodesAsync() => Caller.GetAsync<List<string>>($"{RootPath}/statuscodes")!;

    public Task<List<string>> GetEndpointsAsync(BaseRequestDto query) => Caller.GetByBodyAsync<List<string>>($"{RootPath}/endpointList", query)!;

    public Task<List<string>> GetExceptionTypesAsync(BaseRequestDto query) => Caller.GetByBodyAsync<List<string>>($"{RootPath}/errorTypes", query)!;

    public Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceListAsync(ApmEndpointRequestDto query) => Caller.GetByBodyAsync<PaginatedListBase<SimpleTraceListDto>>($"{RootPath}/simpleTraceList", body: query)!;
}