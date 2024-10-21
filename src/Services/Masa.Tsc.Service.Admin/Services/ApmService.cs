// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class ApmService : ServiceBase
{
    private readonly IMemoryCache _memoryCache;
    public ApmService(IMemoryCache memoryCache) : base("/api/apm")
    {
        _memoryCache = memoryCache;
    }

    public async Task<PaginatedListBase<ServiceListDto>> GetServices([FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
    {
        var query = new BaseApmRequestDto
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
            StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus),
        };
        if (await GetApps(query, authClient, pmClient, teamId, project, appType))
            return await apmService.ServicePageAsync(query);

        return default!;
    }

    public async Task<PaginatedListBase<EndpointListDto>> GetEndpoints([FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, string? env, string? service, string? endpoint, string? statusCode, string? textField, string? textValue, string? exType, string? traceId, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
    {
        var query = new ApmEndpointRequestDto
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
        };

        if (await GetApps(query, authClient, pmClient, teamId, project, appType))
            return await apmService.EndpointPageAsync(query);

        return default!;
    }

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

    public async Task<PaginatedListBase<ErrorMessageDto>> GetErrors([FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, string? env, string? service, string? endpoint, string? exType, string? textField, string? textValue, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc, string? traceId)
    {
        var query = new ApmEndpointRequestDto
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
        };

        if (await GetApps(query, authClient, pmClient, teamId, project, appType))
            return await apmService.ErrorMessagePageAsync(query);

        return default!;
    }

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

    public async Task<PaginatedListBase<LogResponseDto>> GetLogList([FromServices] ILogService logService, IAuthClient authClient, IPmClient pmClient, [FromBody] BaseRequestDto query, [FromQuery] Guid teamId, [FromQuery] string project, [FromQuery] AppTypes? appType, [FromQuery] bool ignoreTeam = false)
    {
        var apmQuery = new BaseApmRequestDto
        {
            Env = GetEnv(query.Conditions.FirstOrDefault(t => t.Name == StorageConst.Current.Environment)?.Value?.ToString()),
            Service = string.IsNullOrEmpty(query.Service) ? query.Conditions.FirstOrDefault(t => t.Name == StorageConst.Current.ServiceName)?.Value?.ToString() : query.Service,
        };

        if (ignoreTeam)
            return await logService.ListAsync(query);

        if (await GetApps(apmQuery, authClient, pmClient, teamId, project, appType))
        {
            if (string.IsNullOrEmpty(apmQuery.Service))
            {
                query.Service = default!;
                var list = query.Conditions?.ToList() ?? new List<FieldConditionDto>();
                list.RemoveAll(t => t.Name == StorageConst.Current.ServiceName);
                list.Add(new FieldConditionDto
                {
                    Name = StorageConst.Current.ServiceName,
                    Type = ConditionTypes.In,
                    Value = apmQuery.AppIds
                });
                query.Conditions = list;
            }
            return await logService.ListAsync(query);
        }

        return default!;
    }

    public async Task<PhoneModelDto> GetModel([FromServices] IApmService apmService, string brand, string model)
        => await apmService.GetDeviceModelAsync(brand, model);

    public async Task<Dictionary<string, List<EnviromentAppDto>>> GetEnviromentService([FromServices] IApmService apmService, [FromServices] IAuthClient authClient, [FromServices] IPmClient pmClient, IMultiEnvironmentContext multiEnvironmentContext, Guid teamId, string start, string end)
    {
        var data = await apmService.GetEnviromentServices(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime()
        });
        //#if RELEASE
        {
            var teamData = await GetTeamAllEnvAppsAsync(authClient, pmClient, teamId, data.Keys);
            if (teamData.Count == 0)
                return new Dictionary<string, List<EnviromentAppDto>> { { multiEnvironmentContext.CurrentEnvironment, new() } };
            var result = new Dictionary<string, List<EnviromentAppDto>>();
            foreach (var env in teamData.Keys)
            {
                if (!data.ContainsKey(env)) continue;
                result.Add(env, teamData[env].Where(app => data[env].Contains(app.AppId)).ToList());
            }
            if (result.Count == 0)
            {
                result.Add(multiEnvironmentContext.CurrentEnvironment, new());
            }
            return result;
        }
        //#endif
        //return data;
    }

    private async Task<bool> GetApps<Request>(Request request, IAuthClient authClient, IPmClient pmClient, Guid teamId, string? project, AppTypes? appType) where Request : BaseApmRequestDto
    {
        if (string.IsNullOrEmpty(request.Env))
            return false;
        var aaaa = await GetTeamAppsAsync(authClient, pmClient, teamId, request.Env!);
        if (aaaa == null || aaaa.Count == 0)
            return false;

        IEnumerable<EnviromentAppDto> filters = aaaa.First().Value;
        project = project?.ToLower();
        if (!string.IsNullOrEmpty(project))
        {
            project = project.ToLower();
            if (!string.IsNullOrEmpty(project))
                filters = filters.Where(p => p.ProjectId.ToLower() == project);
        }
        if (appType.HasValue)
            filters = filters.Where(p => p.AppType == appType.Value);

        if (!string.IsNullOrEmpty(request.Service))
            return filters.Any(app => app.AppId == request.Service);

        request.AppIds = filters.Select(app => app.AppId).ToList();

        return request.AppIds.Any();
    }

    private async Task<Dictionary<string, List<EnviromentAppDto>>> GetTeamAppsAsync(IAuthClient authClient, IPmClient pmClient, Guid teamId, string env)
    {
        var result = new Dictionary<string, List<EnviromentAppDto>>();
        if (teamId == Guid.Empty || string.IsNullOrEmpty(env))
            return result;

        var cacheKey = $"env-app_{teamId}_{env}";
        if (_memoryCache.TryGetValue(cacheKey, out result))
            return result!;
        result = new();
        var team = await authClient.TeamService.GetDetailAsync(teamId);
        if (team == null) return result;

        var projects = await pmClient.ProjectService.GetListByTeamIdsAsync(new List<Guid> { teamId }, env);
        if (projects == null || projects.Count == 0) return result;
        var apps = await pmClient.AppService.GetListByProjectIdsAsync(projects.Select(item => item.Id).ToList());
        result.Add(env, apps.Select(item => new EnviromentAppDto
        {
            AppId = item.Identity,
            ProjectId = projects.Find(p => p.Id == item.ProjectId)?.Identity!,
            AppType = item.Type
        }).ToList());
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    private async Task<Dictionary<string, List<EnviromentAppDto>>> GetTeamAllEnvAppsAsync(IAuthClient authClient, IPmClient pmClient, Guid teamId, IEnumerable<string> envs)
    {
        var result = new Dictionary<string, List<EnviromentAppDto>>();
        foreach (var env in envs)
        {
            var item = await GetTeamAppsAsync(authClient, pmClient, teamId, env);
            if (item != null && item.Count > 0)
                result.Add(item.First().Key, item.First().Value);
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