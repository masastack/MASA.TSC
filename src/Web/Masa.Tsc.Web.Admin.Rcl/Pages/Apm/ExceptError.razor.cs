// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class ExceptError
{
    private async Task LoadASync(SearchData? searchData = default)
    {
        if (searchData != null)
            Search = searchData;
        isTableLoading = true;
        var result = await ApiCaller.ExceptErrorService.ListAsync(new RequestExceptErrorQuery
        {
            Page = page,
            PageSize = defaultSize,
            Environment = Search.Environment!,
            Project = Search.Project!,
            Service = Search.Service!,
            Type = Search.ExceptionType
        });
        data.Clear();
        total = 0;
        if (result != null)
        {
            total = (int)result.Total;
            data = result.Result ?? new();
        }
        isTableLoading = false;
    }

    protected override bool IsPage => true;

    int defaultSize = 20;
    int total = 0;
    bool isTableLoading = true;
    List<ExceptErrorDto> data = new();
    string sortFiled = "";
    bool sortBy = false;
    int page = 1;

    private List<DataTableHeader<ExceptErrorDto>> headers => new()
    {
        new() { Text = I18n.Apm("Search.Environment"), Value = nameof(ExceptErrorDto.Environment),Width=150,Fixed= DataTableFixed.Left},
        new() { Text = I18n.Apm("Search.Project"), Value = nameof(ExceptErrorDto.Project),Width=150,Fixed= DataTableFixed.Left},
        new() { Text = I18n.Apm("Search.Service"), Value =nameof(ExceptErrorDto.Service),Width=200,Fixed= DataTableFixed.Left},
        new() { Text = I18n.Apm("Error.List.Type"), Value =nameof(ExceptErrorDto.Type),Width=200},
        new() { Text = I18n.Apm("Error.List.Message"), Value = nameof(ExceptErrorDto.Message),Groupable=true},
        new() { Text = I18n.T("Comments"), Value = nameof(ExceptErrorDto.Comment)},
        new() { Text = I18n.T("CreationTime"), Value = nameof(ExceptErrorDto.CreationTime), Width = 200},
        new(){  Text= I18n.T("Action"),Value="Action" ,Fixed= DataTableFixed.Right,Width=150},
    };

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

    private async Task DeleteAsync(string id)
    {
        if (!await base.PopupService.ConfirmAsync("确定要删除该数据吗？", AlertTypes.Warning))
            return;
        await ApiCaller.ExceptErrorService.RemoveAsync(id);
        await PopupService.EnqueueSnackbarAsync(I18n.Dashboard("删除成功"), AlertTypes.Success);
        await LoadASync();
        StateHasChanged();
    }

}
