// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class ApmService : ServiceBase
{
    private readonly IMemoryCache _memoryCache;
    public ApmService(IMemoryCache memoryCache) : base("/api/apm")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        _memoryCache = memoryCache;
    }

    public async Task<PaginatedListBase<ServiceListDto>> GetServices([FromServices] IServiceProvider serviceProvider, [FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, string? env, string? service, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
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
        {
            //return await serviceProvider.GetCubeApmService().ServicePageAsync(query);
            return await apmService.ServicePageAsync(query);
        }

        return default!;
    }

    public async Task<PaginatedListBase<EndpointListDto>> GetEndpoints([FromServices] IServiceProvider serviceProvider, [FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, string? env, string? service, string? endpoint, string? statusCode, string? textField, string? textValue, string? exType, string? traceId, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc)
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
        {
            //return await serviceProvider.GetCubeApmService().EndpointPageAsync(query);
            return await apmService.EndpointPageAsync(query);
        }


        return default!;
    }

    public async Task<IEnumerable<ChartLineDto>> GetCharts([FromServices] IServiceProvider serviceProvider, [FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, string? method, ComparisonTypes? comparisonType, string? queries)
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
        return await serviceProvider.GetCubeApmService().ChartDataAsync(queryDto);
        //return await apmService.ChartDataAsync(queryDto);
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

    public async Task<PaginatedListBase<ErrorMessageDto>> GetErrors([FromServices] IApmService apmService, IAuthClient authClient, IPmClient pmClient, int page, int pageSize, string start, string end, Guid teamId, string project, AppTypes? appType, bool ignoreTeam, string? env, string? service, string? endpoint, string? exType, string? textField, string? textValue, ComparisonTypes? comparisonType, string? queries, string? orderField, bool? isDesc, string? traceId, bool filter = true)
    {
        var query = new ApmErrorRequestDto
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
            TraceId = traceId!,
            Filter = filter
        };

        if (ignoreTeam || await GetApps(query, authClient, pmClient, teamId, project, appType))
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

    public async Task<IEnumerable<ChartLineCountDto>> GetErrorChart([FromServices] IApmService apmService, string start, string end, string? env, string? service, string? endpoint, ComparisonTypes? comparisonType, string? queries, string? exType, string? exMessage, bool filter = true)
         => await apmService.GetErrorChartAsync(new ApmErrorRequestDto
         {
             Start = start.ParseUTCTime(),
             End = end.ParseUTCTime(),
             Env = GetEnv(env),
             ComparisonType = comparisonType,
             Queries = queries,
             ExType = exType!,
             ExMessage = exMessage!,
             Service = service,
             Endpoint = endpoint!,
             StatusCodes = string.Join(',', ConfigConst.TraceErrorStatus),
             Filter = filter
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

    public async Task<PaginatedListBase<TraceResponseDto>> AddTraceList([FromServices] ITraceService traceService, [FromBody] BaseRequestDto query)
        => await traceService.ListAsync(query);

    public async Task<PaginatedListBase<LogResponseDto>> AddLogList([FromServices] ILogService logService, IAuthClient authClient, IPmClient pmClient, [FromBody] BaseRequestDto query, [FromQuery] Guid teamId, [FromQuery] string project, [FromQuery] AppTypes? appType, [FromQuery] bool ignoreTeam = false)
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

    public async Task<Dictionary<string, List<EnvironmentAppDto>>> GetEnvironmentService([FromServices] IApmService apmService
        , [FromServices] IAuthClient authClient
        , [FromServices] IPmClient pmClient
        , IMultiEnvironmentContext multiEnvironmentContext, Guid teamId, string start, string end, bool ignoreTeam = false)
    {
        var data = await apmService.GetEnvironmentServices(new BaseApmRequestDto
        {
            Start = start.ParseUTCTime(),
            End = end.ParseUTCTime()
        });

        var teamData = await GetTeamAllEnvAppsAsync(authClient, pmClient, teamId, data.Keys, ignoreTeam);
        if (teamData.Count == 0)
            return new Dictionary<string, List<EnvironmentAppDto>> { { multiEnvironmentContext.CurrentEnvironment, new() } };
        var result = new Dictionary<string, List<EnvironmentAppDto>>();
        foreach (var env in teamData.Keys)
        {
            if (!ignoreTeam && !data.ContainsKey(env)) continue;
            result.Add(env, teamData[env].Where(app => data[env].Contains(app.AppId)).ToList());
        }
        if (result.Count == 0)
        {
            result.Add(multiEnvironmentContext.CurrentEnvironment, new());
        }
        return result;
    }

    private async Task<bool> GetApps<Request>(Request request, IAuthClient authClient, IPmClient pmClient, Guid teamId, string? project, AppTypes? appType) where Request : BaseApmRequestDto
    {
        if (string.IsNullOrEmpty(request.Env))
            return false;
        var apps = await GetTeamAppsAsync(authClient, pmClient, request.Env!, teamId);
        if (apps == null || apps.Count == 0)
            return false;

        IEnumerable<EnvironmentAppDto> filters = apps.First().Value;
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

    private async Task<Dictionary<string, List<EnvironmentAppDto>>> GetTeamAppsAsync(IAuthClient authClient, IPmClient pmClient, string env, params Guid[] teamIds)
    {
        var result = new Dictionary<string, List<EnvironmentAppDto>>();
        if (teamIds.Length == 0 || teamIds.All(teamId => teamId == Guid.Empty) || string.IsNullOrEmpty(env))
            return result;

        var cacheKey = $"env-app_{(teamIds.Length > 1 ? "all" : teamIds[0])}_{env}";
        if (_memoryCache.TryGetValue(cacheKey, out result))
            return result!;
        result = new();

        var projects = await pmClient.ProjectService.GetListByTeamIdsAsync([.. teamIds], env);
        if (projects == null || projects.Count == 0) return result;
        var apps = await pmClient.AppService.GetListByProjectIdsAsync(projects.Select(item => item.Id).ToList());
        result.Add(env, apps.Select(item => new EnvironmentAppDto
        {
            AppId = item.Identity,
            ProjectId = projects.Find(p => p.Id == item.ProjectId)?.Identity!,
            AppType = item.Type,
            AppDescription = item.Description
        }).ToList());
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    private async Task<Dictionary<string, List<EnvironmentAppDto>>> GetTeamAllEnvAppsAsync(IAuthClient authClient, IPmClient pmClient, Guid teamId, IEnumerable<string> envs, bool ignoreTeam = false)
    {
        var result = new Dictionary<string, List<EnvironmentAppDto>>();
        foreach (var env in envs)
        {
            var teamIds = new[] { teamId };
            if (ignoreTeam)
            {
                var teams = await authClient.TeamService.GetAllAsync(env);
                if (teams == null || !teams.Any()) continue;
                teamIds = teams.Select(t => t.Id).ToArray();
            }

            var item = await GetTeamAppsAsync(authClient, pmClient, env, teamIds);
            if (item != null && item.Count > 0)
                result.Add(item.First().Key, item.First().Value);
        }
        return result;
    }

    public Task<PaginatedListBase<SimpleTraceListDto>> AddSimpleTraceList([FromServices] IApmService apmService, [FromBody] ApmTraceLatencyRequestDto query)
        => apmService.GetSimpleTraceListAsync(query);

    public Task<List<string>> AddEndpointList([FromServices] IApmService apmService, [FromBody] BaseApmRequestDto query)
        => apmService.GetEndpointsAsync(query);

    public Task<List<string>> GetStatusCodes([FromServices] IApmService apmService)
        => apmService.GetStatusCodesAsync();

    public Task<List<string>> AddErrorTypes([FromServices] IApmService apmService, [FromBody] BaseApmRequestDto query)
        => apmService.GetErrorTypesAsync(query);

    private static string? GetEnv(string? env) => string.Equals("all", env, StringComparison.CurrentCultureIgnoreCase) ? default : env;
}