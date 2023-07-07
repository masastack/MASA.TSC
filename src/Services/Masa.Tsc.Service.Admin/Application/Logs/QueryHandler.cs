// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public class QueryHandler : EnvQueryHandler
{
    private readonly ILogService _logService;

    public QueryHandler(ILogService logService, IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IMultiEnvironmentContext multiEnvironment)
        : base(masaStackConfig, environment, multiEnvironment)
    {
        _logService = logService;
    }

    [EventHandler]
    public async Task AggregateAsync(LogAggQuery query)
    {
        query.Data.SetValues();
        query.Data.SetEnv(GetServiceEnvironmentName(query.Data.Service));
        query.Result = await _logService.AggregateAsync(query.Data);
    }

    [EventHandler]
    public async Task GetLatestDataAsync(LatestLogQuery queryData)
    {
        var query = new BaseRequestDto
        {
            Start = queryData.Start,
            End = queryData.End,
            Keyword = queryData.Query,
            Page = 1,
            PageSize = 1,
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = !queryData.IsDesc }
        };

        var env = GetServiceEnvironmentName(string.Empty!);
        query.SetEnv(env);
        var data = await _logService.ListAsync(query);

        queryData.Result = data?.Result?.FirstOrDefault()!;
    }

    [EventHandler]
    public async Task GetMappingAsync(LogFieldQuery query)
    {
        query.Result = await _logService.GetMappingAsync();
        if (query.Result == null)
            query.Result = Array.Empty<MappingResponseDto>();
    }

    [EventHandler]
    public async Task GetPageListAsync(LogsQuery queryData)
    {
        bool isSkipEnv = false;
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(queryData.JobTaskId))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Attributes.TaskId.keyword",
                Type = ConditionTypes.Equal,
                Value = queryData.JobTaskId
            });
            isSkipEnv = true;
        }

        if (!string.IsNullOrEmpty(queryData.SpanId))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "SpanId.keyword",
                Type = ConditionTypes.Equal,
                Value = queryData.SpanId
            });
        }

        if (!string.IsNullOrEmpty(queryData.LogLevel))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "SeverityText",
                Type = ConditionTypes.Equal,
                Value = queryData.LogLevel
            });
        }

        bool isRawQuery = (queryData.Query?.IndexOfAny(new char[] { '{', '}' }) ?? -1) >= 0;
        var query = new BaseRequestDto
        {
            Service = queryData.Service!,
            Start = queryData.Start,
            End = queryData.End,
            Keyword = isRawQuery ? string.Empty : queryData.Query!,
            RawQuery = isRawQuery ? queryData.Query! : string.Empty,
            Page = queryData.Page,
            PageSize = queryData.Size,
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = queryData.IsDesc },
            Conditions = conditions
        };
        if (!isSkipEnv && !isRawQuery)
        {
            var env = GetServiceEnvironmentName(queryData.Service!);
            query.SetEnv(env);
        }

        var data = await _logService.ListAsync(query);
        data ??= new PaginatedListBase<LogResponseDto>();
        queryData.Result = data;
    }

    [EventHandler]
    public async Task GetErrorTypesAsync(LogErrorTypesQuery query)
    {
        query.Result = await _logService.GetElasticClient().GetLogErrorTypesAsync(query.Service, query.Start, query.End, GetServiceEnvironmentName(query.Service));
    }
}