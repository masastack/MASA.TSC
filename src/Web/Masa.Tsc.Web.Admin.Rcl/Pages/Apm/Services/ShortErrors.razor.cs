// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Security.Cryptography;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Services;

public partial class ShortErrors
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    [Parameter]
    public MetricTypes MetricType { get; set; }

    [Parameter]
    public int ApiIndex { get; set; }

    private List<DataTableHeader<ErrorMessageDto>> headers => new()
    {
        new() { Text = I18n.Apm(nameof(ErrorMessageDto.Type)), Value = nameof(ListChartData.Name)},
        new() { Text = I18n.Apm(nameof(ErrorMessageDto.LastTime)), Value = nameof(ListChartData.Latency) },
        new() { Text = I18n.Apm(nameof(ErrorMessageDto.Total)), Value = nameof(ListChartData.Throughput)}
    };

    private int defaultSize = 5;
    private int total = 0;
    private int page = 1;
    private List<ErrorMessageDto> data = new();
    private bool isTableLoading = false;
    private string? sortFiled;
    private bool? sortBy;
    private string lastKey = string.Empty;

    private async Task OnTableOptionsChanged(DataOptions sort)
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

    private void OpenAsync(ErrorMessageDto item)
    {
    }

    protected override async Task OnParametersSetAsync()
    {
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key)
        {
            lastKey = key;
            await LoadASync();
        }
        await base.OnParametersSetAsync();
    }

    private async Task LoadASync()
    {
        if (isTableLoading)
        {
            return;
        }
        await LoadPageDataAsync();
        isTableLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        if (isTableLoading) return;
        isTableLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = SearchData.Start,
            End = SearchData.End,
            OrderField = sortFiled,
            Service = SearchData.Service,
            Env = SearchData.Enviroment,
            IsDesc = sortBy
        };
        var result = await ApiCaller.ApmService.GetErrorsPageAsync(query);
        data.Clear();
        if (result.Result != null && result.Result.Any())
        {
            data.AddRange(result.Result);
        }
        total = (int)result.Total;
    }
}
