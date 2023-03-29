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
        if (query.Data.Conditions != null)
        {
            foreach (var item in query.Data.Conditions)
            {
                if (item.Type == ConditionTypes.In && item.Value is JsonElement json)
                {
                    item.Value = json.EnumerateArray().Select(value => value.ToString());
                }
            }
        }

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

        var data = await _logService.ListAsync(new BaseRequestDto
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
        });

        data ??= new PaginatedListBase<LogResponseDto>();

        queryData.Result = data;
    }
}