// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class TscLog
{
    private int _lastedDuration = 1;
    private List<KeyValuePair<int, string>> _dicDurations = TimeSeries;
    private List<JsonElement> _data = default!;
    private int _totalPage = 1;
    private bool _isWordBreak = false;
    private bool _isDesc = true;
    private int _currentPage = 1;
    private int _pageSize = 10;
    private string _queryStr = default!;
    private string _msg = "Loading ...";
    private DateTime? _start = DateTime.Now.Date, _end = DateTime.Now;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _msg = "query not find data";
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateWordBreakAsync(bool value)
    {
        _isWordBreak = value;
        await Task.CompletedTask;
    }

    private async Task UpdateSortingAsync()
    {
        _isDesc = !_isDesc;
        await RefreshAsync();
        await Task.CompletedTask;
    }

    private async Task UpdateDurationAsync(KeyValuePair<int, string> item)
    {
        _lastedDuration = item.Key;
        await RefreshAsync();
        await Task.CompletedTask;
    }

    private async Task PageChangeAsync(int value)
    {
        _currentPage = value;
        await RefreshAsync();
        await Task.CompletedTask;
    }

    private async Task QueryAsync()
    {
        _currentPage = 1;
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        CheckTime();
        var query = new LogPageQueryDto
        {
            PageSize = _pageSize,
            Start = TimeZoneInfo.ConvertTime(_start!.Value, CurrentTimeZone),
            End = TimeZoneInfo.ConvertTime(_end!.Value, CurrentTimeZone),
            Page = _currentPage,
            Duration = _lastedDuration.ToString(),
            Sorting = !_isDesc ? "asc" : "desc",
            Query = _queryStr
        };
        var pageData = await ApiCaller.LogService.GetPageAsync(query);
        if (pageData.Items != null && pageData.Items.Any())
        {
            _data = pageData.Items.Select(item => (JsonElement)item).ToList();
        }
        else
        {
            _data = default!;
        }
        var num = pageData.Total % query.PageSize;
        _totalPage = (int)(pageData.Total / query.PageSize) + (num > 0 ? 1 : 0);
        StateHasChanged();
    }

    private void CheckTime()
    {
        if (!_start.HasValue || !_end.HasValue)
        {
            var time = TimeZoneInfo.ConvertTime(DateTime.Now, CurrentTimeZone);
            _start = time.Date;
            _end = time;
        }
    }
}