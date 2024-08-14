﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Logs
{
    [Parameter]
    public string TaskId { get; set; }

    protected override bool IsPage => true;

    private List<DataTableHeader<LogResponseDto>> headers => new()
    {
        new() { Text = I18n.Apm("Log.List.Timestamp"), Value = nameof(LogResponseDto.Timestamp),Fixed = DataTableFixed.Left},
        new() { Text = I18n.Apm("Log.List.Environment"), Value ="Resource.service.namespace",Fixed = DataTableFixed.Left},
        new() { Text = I18n.Apm("Log.List.ServiceName"), Value ="Resource.service.name",Fixed = DataTableFixed.Left },
        new() { Text = I18n.Apm("Log.List.SeverityText"), Value = nameof(LogResponseDto.SeverityText),Fixed = DataTableFixed.Left},
        new() { Text = I18n.Apm("Log.List.TraceId"), Value = nameof(LogResponseDto.TraceId)},
        new() { Text = I18n.Apm("Log.List.SpanId"), Value = nameof(LogResponseDto.SpanId)},
        new() { Text = I18n.Apm("Log.List.Body"), Value = nameof(LogResponseDto.Body)},
        new() { Text = I18n.Apm("Log.List.ExceptionType"), Value = "Attributes.exception.type"}
    };

    private int defaultSize = 50;
    private int total = 0;
    private int page = 1;
    private List<LogResponseDto> data = new();
    private bool isTableLoading = false;
    private string? sortFiled = nameof(LogResponseDto.Timestamp);
    private bool? sortBy = true;
    private bool dialogShow = false;
    private LogResponseDto current = null;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (!string.IsNullOrEmpty(TaskId))
        {
            Search.TextField = StorageConst.Current.Log.TaskId;
            Search.TextValue = TaskId;
            StateHasChanged();
        }
    }

    public async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy[0];
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc[0];
        else
            sortBy = default;
        await LoadASync();
    }

    private void OpenAsync(LogResponseDto item)
    {
        current = item;
        dialogShow = true;
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadASync();
    }

    private async Task LoadASync(SearchData data = null!)
    {
        isTableLoading = true;
        if (data != null)
        {
            total = 0;
            page = 1;
            Search = data;
        }
        StateHasChanged();
        await LoadPageDataAsync();
        isTableLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new BaseRequestDto()
        {
            Start = Search.Start,
            End = Search.End,
            Service = Search.Service!,
            Page = page,
            Sort = new FieldOrderDto
            {
                Name = sortFiled!,
                IsDesc = sortBy ?? false
            },
            PageSize = defaultSize
        };
        var list = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(Search.Environment))
        {
            list.Add(new FieldConditionDto { Name = StorageConst.Current.Environment, Value = Search.Environment, Type = ConditionTypes.Equal });
        }
        if (!string.IsNullOrEmpty(Search.ExceptionType))
        {
            list.Add(new FieldConditionDto { Name = StorageConst.Current.ExceptionType, Value = Search.ExceptionType, Type = ConditionTypes.Equal });
        }
        if (!string.IsNullOrEmpty(Search.TextField) && !string.IsNullOrEmpty(Search.TextValue))
        {
            if (Search.TextField == StorageConst.Current.ExceptionMessage || Search.TextField == StorageConst.Current.Log.Body)
            {
                list.Add(new FieldConditionDto { Name = Search.TextField, Value = Search.TextValue, Type = ConditionTypes.Regex });
            }
            else
            {
                list.Add(new FieldConditionDto { Name = Search.TextField, Value = Search.TextValue, Type = ConditionTypes.Equal });
            }
        }
        query.Conditions = list;
        var result = await ApiCaller.ApmService.GetLogListAsync(query);
        data.Clear();
        if (result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result);
        }
        if (total == 0)
            total = (int)result.Total;
    }
}