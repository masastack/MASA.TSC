// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch : IDisposable
{
    [Parameter]
    public Func<string, string, string, string, DateTime, DateTime, Task> OnSearchAsync { get; set; }

    private bool _isLoading = false;
    private bool _isSearching = false;
    private List<string> _services = default!;
    private List<string> _instances = default!;
    private List<string> _endpoints = default!;

    private string _service = default!;
    private string _instance = default!;
    private string _endpoint = default!;
    private string _keyword = default!;
    private DateTime? _start;
    private DateTime? _end;
    private bool hasChange = true;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _instances = new List<string>();
        _endpoints = new List<string>();
        await SearchAsync();
    }

    private async Task UpdateSearchInputAsync(int type, string val)
    {
        if (_isLoading || string.IsNullOrEmpty(val) || _keyword == val)
        {
            return;
        }

        _keyword = val;
        _isLoading = true;
        StateHasChanged();
        CheckTime();
        var query = new SimpleAggregateRequestDto
        {
            End = _end!.Value,
            Start = _start!.Value,
            Keyword = _keyword,
            MaxCount = 10,
            Type = AggregateTypes.GroupBy,
        };

        IEnumerable<string> data;
        switch (type)
        {
            case 1:
                data = await ApiCaller.TraceService.GetAttrValuesAsync(query)!;
                if (data != null && data.Any())
                    _services = data.ToList();
                else
                    _services.Clear();
                break;
            case 2:
                query.Service = _service;
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _instances = data.ToList();
                break;
            case 3:
                query.Service = _service;
                query.Instance = _instance;
                data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                _endpoints = data.ToList();
                break;
        }
        _isLoading = false;
        StateHasChanged();
    }

    private async Task UpdateItemAsync(int type, string value)
    {
        if (_isLoading || string.IsNullOrEmpty(value))
            return;

        _isLoading = true;
        StateHasChanged();
        CheckTime();
        hasChange = true;

        var query = new SimpleAggregateRequestDto
        {
            End = _end!.Value,
            Start = _start!.Value,
            Keyword = _keyword,
            MaxCount = 10,
            Type = AggregateTypes.GroupBy,
        };
        IEnumerable<string> data;
        switch (type)
        {
            case 1:
            case 2:
                _endpoint = default!;
                if (type == 1)
                {
                    _instance = default!;
                    _instance = value;
                    query.Service = _service;
                    data = await ApiCaller.TraceService.GetAttrValuesAsync(query);
                    if (data != null && data.Any())
                        _instances = data.ToList();
                    else
                        _instances.Clear();
                }
                else
                {
                    _instance = value;
                }

                query.Instance = _instance;
                data = await ApiCaller.TraceService.GetAttrValuesAsync(query);
                if (data != null && data.Any())
                    _endpoints = data.ToList();
                else
                    _endpoints.Clear();
                break;
            case 3:
                _endpoint = value;
                break;
        }

        _isLoading = false;
        StateHasChanged();
    }

    private async Task SearchAsync()
    {
        if (_isSearching)
            return;
        _isSearching = true;
        StateHasChanged();
        try
        {
            if (hasChange)
            {
                CheckTime();
                var query = new SimpleAggregateRequestDto
                {
                    End = _end!.Value,
                    Start = _start!.Value,
                    Keyword = _keyword,
                    MaxCount = 10,
                    Type = AggregateTypes.GroupBy,
                };
                var list = await ApiCaller.TraceService.GetAttrValuesAsync(query);
                _services = list?.ToList() ?? new();

                if (OnSearchAsync != null)
                {
                    await OnSearchAsync.Invoke(_service, _instance, _endpoint, _keyword, _start.Value, _end.Value);
                }
            }
        }
        finally
        {
            hasChange = false;
            _isSearching = false;
            StateHasChanged();
        }
    }

    private void ValueChange(string name, object value)
    {
        hasChange = true;
        switch (name)
        {
            case "keyword":
                _keyword = value.ToString()!;
                break;
            case "start":
                _start = (DateTime?)value;
                break;
            case "end":
                _end = (DateTime?)value;
                break;
            default:
                hasChange = false;
                break;
        }
        if (hasChange)
            StateHasChanged();
    }

    private void CheckTime()
    {
        if (!_start.HasValue || !_end.HasValue)
        {
            _end = TimeZoneInfo.ConvertTime(DateTime.UtcNow, CurrentTimeZone);
            _start = TimeZoneInfo.ConvertTime(DateTime.UtcNow.Date, CurrentTimeZone);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        base.Dispose(disposing);
    }
}
