// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public class QueryHandler
{
    private readonly ILogService _logService;

    public QueryHandler(ILogService logService)
    {
        _logService = logService;
    }

    [EventHandler]
    public async Task AggregateAsync(LogAggQuery query)
    {
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
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(queryData.JobTaskId))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Attributes.TaskId.keyword",
                Type = ConditionTypes.Equal,
                Value = queryData.JobTaskId
            });
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

        var data = await _logService.ListAsync(new BaseRequestDto
        {
            Start = queryData.Start,
            End = queryData.End,
            Keyword = queryData.Query,
            Page = queryData.Page,
            PageSize = queryData.Size,
            Sort = new FieldOrderDto { Name = "@timestamp", IsDesc = queryData.IsDesc },
            Conditions = conditions
        });
        if (data == null)
            data = new PaginatedListBase<LogResponseDto>();

        queryData.Result = data;
    }
}