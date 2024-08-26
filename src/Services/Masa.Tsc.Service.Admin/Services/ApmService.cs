// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;

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

    public async Task<PaginatedListBase<EndpointListDto>> GetEndpoints([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, string? endpoint, string? statusCode, string? textField, string? textValue, string? exType, string? traceId, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
        => await apmService.EndpointPageAsync(new ApmEndpointRequestDto
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
            Endpoint = endpoint!,
            TextField = textField!,
            TextValue = textValue!,
            StatusCode = statusCode!,
            ExType = exType!,
            TraceId = traceId!,
            Service = service,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<IEnumerable<ChartLineDto>> GetCharts([FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, string? method, ComparisonTypes? comparisonType, string? queries)
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
                Endpoint = endpoint.Equals("@all", StringComparison.InvariantCultureIgnoreCase) ? "" : endpoint!,
                Method = method!
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

    public async Task<EndpointLatencyDistributionDto> GetLatencyDistributions([FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, string? method, string? textField, string? textValue, string? exType, string? traceId)
        => await apmService.EndpointLatencyDistributionAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            Service = service,
            Endpoint = endpoint!,
            Method = method!,
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus)
        });

    public async Task<PaginatedListBase<ErrorMessageDto>> GetErrors([FromServices] IApmService apmService, int page, int pageSize, string start, string end, string? env, string? service, string? endpoint, string? exType, string? textField, string? textValue, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc, string? traceId)
        => await apmService.ErrorMessagePageAsync(new ApmEndpointRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime(),
            Env = GetEnv(env),
            Endpoint = endpoint!,
            Queries = queries,
            OrderField = orderField,
            IsDesc = isDesc,
            Page = page,
            ExType = exType!,
            TextField = textField!,
            TextValue = textValue!,
            PageSize = pageSize,
            Service = service,
            TraceId = traceId!
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

    public async Task<Dictionary<string, List<string>>> GetEnviromentService([FromServices] IApmService apmService, [FromServices] IAuthClient authClient, [FromServices] IPmClient pmClient, IMultiEnvironmentContext multiEnvironmentContext, Guid teamId, string start, string end)
    {
        var data = await apmService.GetEnviromentServices(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime()
        });
#if RELEASE
        {
            var teamData = await GetTeamAppsAsync(authClient, pmClient, teamId, data.Keys);
            if (teamData.Count == 0)
                return new Dictionary<string, List<string>> { { multiEnvironmentContext.CurrentEnvironment, new() } };
            var result = new Dictionary<string, List<string>>();
            foreach (var env in teamData.Keys)
            {
                if (!data.ContainsKey(env)) continue;
                result.Add(env, data[env].Where(appid => teamData[env].Contains(appid)).ToList());
            }
            if (result.Count == 0)
            {
                result.Add(multiEnvironmentContext.CurrentEnvironment, new());
            }
            return result;
        }
#endif
        return data;
    }

    private async Task<Dictionary<string, List<string>>> GetTeamAppsAsync(IAuthClient authClient, IPmClient pmClient, Guid teamId, IEnumerable<string> envs)
    {
        var result = new Dictionary<string, List<string>>();
        if (teamId == Guid.Empty || envs == null || !envs.Any())
            return result;
        var team = await authClient.TeamService.GetDetailAsync(teamId);
        if (team == null) return result;
        foreach (var env in envs)
        {
            var projects = await pmClient.ProjectService.GetListByTeamIdsAsync(new List<Guid> { teamId }, env);
            if (projects == null) continue;
            var apps = await pmClient.AppService.GetListByProjectIdsAsync(projects.Select(item => item.Id).ToList());
            //var apps = await pmClient.ProjectService.GetProjectAppsAsync(env);
            result.Add(env, apps.Select(item => item.Identity).ToList());
        }

        return result;
    }

    public Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceList([FromServices] IApmService apmService, [FromBody] ApmEndpointRequestDto query)
        => apmService.GetSimpleTraceListAsync(query);

    public Task<List<string>> GetEndpointList([FromServices] IApmService apmService, [FromBody] BaseApmRequestDto query)
        => apmService.GetEndpointsAsync(query);

    public Task<List<string>> GetStatusCodes([FromServices] IApmService apmService)
        => apmService.GetStatusCodesAsync();

    public Task<List<string>> GetErrorTypes([FromServices] IApmService apmService, [FromBody] BaseApmRequestDto query)
        => apmService.GetErrorTypesAsync(query);

    private static string? GetEnv(string? env) => string.Equals("all", env, StringComparison.CurrentCultureIgnoreCase) ? default : env;
}