// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceSearch
{
    [Parameter]
    public Func<string, string, string, string, DateTime, DateTime, Task> OnSearchAsync { get; set; }

    private string _timeStr = "";
    private bool _isLoading = false;

    private List<string> _services = default!;
    private List<string> _instances = default!;
    private List<string> _endpoints = default!;

    private string _service = default!;
    private string _instance = default!;
    private string _endpoint = default!;
    private string _keyword = default!;
    private DateTime _start = default!;
    private DateTime _end = default!;

    private async Task UpdateSearchInputAsync(string val)
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;

        _isLoading = false;
        await Task.CompletedTask;
    }

    private async Task UpdateItemAsync(string item)
    {
        //_model = item;
        //await Task.Delay(100);//We will fix this when transition update to javascript
        //_show = true;
        await Task.CompletedTask;
    }

    private async Task SearchAsync()
    {
        if (OnSearchAsync is not null)
        {
            await OnSearchAsync.Invoke(_service, _instance, _endpoint, _keyword, _start, _end);
        }
    }
}
