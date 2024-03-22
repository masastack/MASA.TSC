// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class Logs
{
    protected override bool IsPage => true;

    private List<DataTableHeader<LogResponseDto>> headers => new()
    {
        new() { Text = I18n.Apm("Log.List.Timestamp"), Value = nameof(LogResponseDto.Timestamp),Fixed = DataTableFixed.Left},
        new() { Text = I18n.Apm("Log.List.Enviroment"), Value ="Resource.service.namespace",Fixed = DataTableFixed.Left},
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
    private string? sortFiled;
    private bool? sortBy;
    private bool dialogShow = false;
    private LogResponseDto current = null;

    private string? exceptionType = null;
    private string? exceptionMessage = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        exceptionType = HttpUtility.ParseQueryString(uri.Query).Get("ex_type");
        exceptionMessage = HttpUtility.ParseQueryString(uri.Query).Get("ex_msg");
        var builder = new StringBuilder(Search.Text);
        if (!string.IsNullOrEmpty(exceptionType))
        {
            builder.Append($" and {StorageConst.ExceptionType}=\"{exceptionType}\"");
        }
        if (builder.Length > 0 && string.IsNullOrEmpty(Search.Text))
            builder.Remove(0, 4);
        Search.Text = builder.ToString();
    }

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
            Search = data;
        StateHasChanged();
        await LoadPageDataAsync();
        isTableLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        isTableLoading = true;
        var query = new LogPageQueryDto()
        {
            Start = Search.Start,
            End = Search.End,
            Service = Search.Service!,
            Page = page,
            Env = Search.Enviroment!,
            PageSize = defaultSize,
            IsDesc = sortBy ?? false,
            SortField = sortFiled!,
            Query = Search.Text,
            IsLimitEnv=false
        };
        var result = await ApiCaller.LogService.GetPageAsync(query);
        data.Clear();
        if (result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result);
        }
        total = (int)result.Total;
    }
}