// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
    private string? sortFiled;
    private bool? sortBy;

    public async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy.First();
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc.First();
        else
            sortBy = default;
        await LoadASync();
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
            page = 1;
            total = 0;
            Search = data;
        }
        StateHasChanged();
        await LoadPageDataAsync();
        isTableLoading = false;
        //StateHasChanged();
        //await LoadChartDataAsync();
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = Search.Start,
            End = Search.End,
            OrderField = sortFiled,
            Env = Search.Environment,
            IsDesc = sortBy,
            Service = Search.Service,
            Queries = Search.Text
        };
        var result = await ApiCaller.ApmService.GetErrorsPageAsync(query);
        data.Clear();
        if (result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result);
        }
        total = (int)result.Total;
    }

    string? type = default, message = default;
    bool showDetail = false;

    private void Show(string? type = default, string? message = default)
    {
        this.type = type;
        this.message = message;
        showDetail = true;
        StateHasChanged();
    }
}
