// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch
{
    [Parameter]
    public Func<string, string, string, string, DateTime, DateTime, Task> OnSearchAsync { get; set; }

    private bool _isLoading = false;
    private List<string> _services = default!;
    private List<string> _instances = default!;
    private List<string> _endpoints = default!;

    private string _service = default!;
    private string _instance = default!;
    private string _endpoint = default!;
    private string _keyword = default!;
    private DateTime? _start;
    private DateTime? _end;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _instances = new List<string>();
        _endpoints = new List<string>();
        await SearchAsync();
    }

    private async Task UpdateSearchInputAsync(int type, string val)
    {
        if (_isLoading || string.IsNullOrEmpty(val))
        {
            return;
        }

        _isLoading = true;
        CheckTime();
        var query = new RequestAttrDataDto
        {
            End = TimeZoneInfo.ConvertTime(_end!.Value, CurrentTimeZone),
            Start = TimeZoneInfo.ConvertTime(_start!.Value, CurrentTimeZone),
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
        CheckTime();
        var query = new RequestAttrDataDto
        {
            End = TimeZoneInfo.ConvertTime(_end!.Value, CurrentTimeZone),
            Start = TimeZoneInfo.ConvertTime(_start!.Value, CurrentTimeZone),
            Max = 10
        };
        IEnumerable<string> data;
        switch (type)
        {
            case 1:
            case 2:
                if (type == 1)
                {
                    query.Query = new Dictionary<string, string> {
                    {"service.name",_service}
                };
                    query.Name = "service.node.name";
                    data = (await ApiCaller.TraceService.GetAttrValuesAsync(query))!;
                    _instances = data.ToList();
                }

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
        CheckTime();
        _services = (await ApiCaller.TraceService.GetAttrValuesAsync(new RequestAttrDataDto
        {
            End = TimeZoneInfo.ConvertTime(_end!.Value, CurrentTimeZone),
            Start = TimeZoneInfo.ConvertTime(_start!.Value, CurrentTimeZone),
            Name = "service.name",
            Max = 10
        })).ToList();

        if (OnSearchAsync is not null)
        {
            await OnSearchAsync.Invoke(_service, _instance, _endpoint, _keyword, TimeZoneInfo.ConvertTime(_start.Value, CurrentTimeZone), TimeZoneInfo.ConvertTime(_end.Value, CurrentTimeZone));
        }
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
