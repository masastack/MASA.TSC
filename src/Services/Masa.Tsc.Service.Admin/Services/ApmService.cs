// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class ApmService : ServiceBase
{
    public ApmService() : base("/api/apm")
    {
    }

    public async Task<PaginatedListBase<ServiceListDto>> GetServices([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.ServicePageAsync(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            Service = service,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<PaginatedListBase<EndpointListDto>> GetEndpoints([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.EndpointPageAsync(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
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

    public async Task<IEnumerable<ChartLineDto>> GetCharts([FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, ComparisonTypes? comparisonType, string? queries)
    {
        BaseApmRequestDto queryDto;

        if (endpoint == null || string.IsNullOrEmpty(service))
        {
            queryDto = new BaseApmRequestDto();
        }
        else
        {
            queryDto = new ApmEndpointRequestDto()
            {
                Endpoint = endpoint.Equals("@all", StringComparison.InvariantCultureIgnoreCase) ? "" : endpoint!
            };
        }
        queryDto.Start = start.ParseUTCTime();
        queryDto.End = end.ParseUTCTime();
        queryDto.Env = GetEnv(env);
        queryDto.ComparisonType = comparisonType;
        queryDto.Queries = queries;
        queryDto.Service = service;
        queryDto.StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus);
        return await apmService.ChartDataAsync(queryDto);
    }

    public async Task<EndpointLatencyDistributionDto> GetLatencyDistributions([FromServices] IApmService apmService, string start, string end, string? env, string? service, string endpoint)
        => await apmService.EndpointLatencyDistributionAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            Service = service,
            Endpoint = endpoint,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<PaginatedListBase<ErrorMessageDto>> GetErrors([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, string? endpoint, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.ErrorMessagePageAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            //Endpoint = endpoint!,
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            Service = service
        });

    public Task<IEnumerable<ChartPointDto>> GetSpanErrors([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => apmService.GetTraceErrorsAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            PageSize = pageSize,
            Service = service
        });

    public async Task<PaginatedListBase<TraceResponseDto>> GetTraceDetail([FromServices] IApmService apmService, int page, string start, string end, string? env, string? service, string endpoint, long? latMin, long? latMax)
        => await apmService.TraceLatencyDetailAsync(new ApmTraceLatencyRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            Page = page,
            Service = service,
            Endpoint = endpoint,
            LatMax = latMax,
            LatMin = latMin
        });

    public async Task<IEnumerable<ChartLineCountDto>> GetErrorChart([FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, ComparisonTypes? comparisonType, string? queries)
         => await apmService.GetErrorChartAsync(new ApmEndpointRequestDto
         {
             Start = start.ParseUTCTime(),
             End = end.ParseUTCTime(),
             Env = GetEnv(env),
             ComparisonType = comparisonType,
             Queries = queries,
             Service = service,
             Endpoint = endpoint!,
             StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
         });

    public async Task<IEnumerable<ChartLineCountDto>> GetLogChart([FromServices] IApmService apmService, string start, string end, string? env, string? service, ComparisonTypes? comparisonType, string? queries)
        => await apmService.GetLogChartAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            ComparisonType = comparisonType,
            Queries = queries,
            Service = service
        });

    public async Task<PaginatedListBase<TraceResponseDto>> GetTraceList([FromServices] ITraceService traceService, [FromBody] BaseRequestDto query)
        => await traceService.ListAsync(query);

    public async Task<PaginatedListBase<LogResponseDto>> GetLogList([FromServices] ILogService logService, [FromBody] BaseRequestDto query)
        => await logService.ListAsync(query);

    public async Task<PhoneModelDto> GetModel([FromServices] IApmService apmService, string brand, string model)
        => await apmService.GetDeviceModelAsync(brand, model);

    public Dictionary<string, List<string>> GetEnviromentService([FromServices] IApmService apmService, string start, string end)
        => apmService.GetEnviromentServices(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime()
        });

    public Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceList([FromServices] IApmService apmService, [FromBody] ApmEndpointRequestDto query)
        => apmService.GetSimpleTraceListAsync(query);

    private static string? GetEnv(string? env) => string.Equals("all", env, StringComparison.CurrentCultureIgnoreCase) ? default : env;
}