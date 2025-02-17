﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Errors
{
    protected override bool IsPage => true;

    private List<DataTableHeader<ErrorMessageDto>> headers => new()
    {
        new() { Text = I18n.Apm("Error.List.LastTime"), Value = nameof(ErrorMessageDto.LastTime) },
        new() { Text = I18n.Apm("Error.List.Type"), Value = nameof(ErrorMessageDto.Type)},
        new() { Text = I18n.Apm("Error.List.Message"), Value =nameof(ErrorMessageDto.Message)},
        new() { Text = I18n.Apm("Error.List.Total"), Value = nameof(ErrorMessageDto.Total)}
    };

    private int defaultSize = 50;
    private int total = 0;
    private int page = 1;
    private List<ErrorMessageDto> data = new();
    private bool isTableLoading = false;
    private string sortFiled = nameof(ErrorMessageDto.Total);
    private bool sortBy = true;
    bool showDetail = false;

    public async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy[0];
        else
            sortFiled = default!;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc[0];
        else
            sortBy = default;
        await LoadASync();
    }

    private async Task OnPageChange((int page, int pageSize) pageData)
    {
        page = pageData.page;
        defaultSize = pageData.pageSize;
        await LoadASync();
        StateHasChanged();
    }

    private async Task LoadASync(SearchData data = null!)
    {
        if (data != null)
        {
            page = 1;
            total = 0;
            Search = data;
        }
        if (!Search.Loaded)
            return;
        isTableLoading = true;
        StateHasChanged();
        await LoadPageDataAsync();
        isTableLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new ApmErrorRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            OrderField = sortFiled,
            Env = Search.Environment,
            IsDesc = sortBy,
            Service = Search.Service,
            ExType = Search.ExceptionType,
            TextField = Search.TextField,
            TextValue = Search.TextValue,
            Filter = Search.EnableExceptError
        };
        var result = await ApiCaller.ApmService.GetErrorsPageAsync(GlobalConfig.CurrentTeamId, query, Search.Project, Search.ServiceType);
        data.Clear();
        total = 0;
        if (result != null)
        {
            total = (int)result.Total;
            if (result.Result != null && result.Result.Any())
                data = result.Result;
        }
    }

    private void Show(string? type = default, string? message = default)
    {
        Search.ExceptionType = type!;
        Search.ExceptionMsg = message!;
        showDetail = true;
        StateHasChanged();
    }

    private void Hide(bool show)
    {
        if (!show)
        {
            Search.ExceptionType = default!;
            Search.ExceptionMsg = default!;
        }
        showDetail = show;
    }
}
