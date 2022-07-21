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
    private DateTime _start = DateTime.Now.Date;
    private DateTime _end = DateTime.Now;


    protected override async Task OnInitializedAsync()
    {
        _services = (await ApiCaller.TraceService.GetAttrValuesAsync(new RequestAttrDataDto
        {
            End = _end,
            Start = _start,
            Name = "service.name",
            Max = 10
        })).ToList();
        _instances = new List<string>();
        _endpoints = new List<string>();
        await base.OnInitializedAsync();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SearchAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateSearchInputAsync(int type, string val)
    {
        if (_isLoading || string.IsNullOrEmpty(val))
        {
            return;
        }

        _isLoading = true;

        var query = new RequestAttrDataDto
        {
            End = _end,
            Start = _start,
            Keyword = val,
            Max = 10
        };

        IEnumerable<string> data;
        switch (type)
        {
            case 1:
                query.Name = "service.name";
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _services = data.ToList();
                break;
            case 2:
                query.Query = new Dictionary<string, string> {
                    {"service.name",_service}
                };
                query.Name = "service.node.name";
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _instances = data.ToList();
                break;
            case 3:
                query.Query = new Dictionary<string, string> {
                    {"service.name",_service},
                    {"service.node.name",_instance}
                };
                query.Name = "transaction.name";
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _endpoints = data.ToList();
                break;
        }
        //StateHasChanged();
        _isLoading = false;
        await Task.CompletedTask;
    }

    private async Task UpdateItemAsync(int type, string value)
    {
        if (_isLoading || string.IsNullOrEmpty(value))
            return;

        _isLoading = true;

        var query = new RequestAttrDataDto
        {
            End = _end,
            Start = _start,
            Max = 10
        };
        IEnumerable<string> data;
        switch (type)
        {
            case 1:
                query.Query = new Dictionary<string, string> {
                    {"service.name",_service}
                };
                query.Name = "service.node.name";
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _instances = data.ToList();
                break;
            case 2:
                query.Query = new Dictionary<string, string> {
                    {"service.name",_service},
                    {"service.node.name",_instance}
                };
                query.Name = "transaction.name";
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _endpoints = data.ToList();
                break;
        }

        _isLoading = false;
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
