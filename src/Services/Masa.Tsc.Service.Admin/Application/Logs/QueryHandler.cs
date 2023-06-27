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
        var data = await _logService.ListAsync(new BaseRequestDto
        {
            Start = queryData.Start,
            End = queryData.End,
            Keyword = queryData.Query,
            Page = 1,
            PageSize = 1,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{
                    Name= ElasticSearchConst.Environment,
                    Type= ConditionTypes.Equal,
                    Value=_multiEnvironment.CurrentEnvironment
                }
            },
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = !queryData.IsDesc }
        });

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
        bool isMasaStack = false;
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(queryData.JobTaskId))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Attributes.TaskId.keyword",
                Type = ConditionTypes.Equal,
                Value = queryData.JobTaskId
            });
            isMasaStack = true;
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

        if (isMasaStack)
        {
            conditions.Add(new FieldConditionDto
            {
                Name = ElasticSearchConst.Environment,
                Type = ConditionTypes.Equal,
                Value = _environment.EnvironmentName
            });
        }
        else
        {
            conditions.Add(new FieldConditionDto
            {
                Name = ElasticSearchConst.Environment,
                Type = ConditionTypes.Equal,
                Value = GetServiceEnvironmentName(queryData.Service!)
            });
        }

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

        query.SetEnv(GetServiceEnvironmentName(queryData.Service!));

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