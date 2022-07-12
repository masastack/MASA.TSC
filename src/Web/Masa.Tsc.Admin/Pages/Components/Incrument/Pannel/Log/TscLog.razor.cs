// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscLog
{
    private int _lastedDuration = 1;

    private List<KeyValuePair<int, string>> _dicDurations = default!;

    private List<JsonElement> _data = default!;

    private int _totalPage = 1;

    private bool _isWordBreak = true;

    private bool _isDesc = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var query = new LogPageQueryDto { 
            PageSize=10,
             Start=DateTime.Now.Date,
             End=DateTime.Now              
        };
        var pageData=await ApiCaller.LogService.GetPageAsync(query);
        _data = pageData.Items.Select(item => (JsonElement)item).ToList();

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateWordBreakAsync(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            _isWordBreak = (bool)e.Value;
        }

        await Task.CompletedTask;
    }

    private async Task UpdateSortingAsync()
    {
        _isDesc = !_isDesc;
        await Task.CompletedTask;
    }

    private async Task UpdateDurationAsync(KeyValuePair<int, string> item)
    {
        await Task.CompletedTask;
    }

    private async Task RefreshAsync()
    {
        await Task.CompletedTask;
    }
}
