// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceDetail
{
    private List<string> _services = default!;
    private DateTime _startTime;
    private string _duration;
    private int _total;
    private StringNumber _tabIndex = "attr";

    private bool _isLoading = true;

    [Parameter]
    public string TraceId { get; set; } = "7834e25540bd5aeb6b1a0ea98a220c43";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var data = await ApiCaller.TraceService.GetAsync(TraceId);
            if (data != null && data.Any())
            {
                _data = data.Select(item => (object)((JsonElement)item).ToKeyValuePairs()!);
            }

            _total = data.Count();
            _startTime = DateTime.Parse(GetDictionaryValue(_data.First(), "@timestamp").ToString()!);
            var duration = Convert.ToInt64(GetDictionaryValue(_data.First(), "transaction.duration.us").ToString()!);
            if (duration / 1000 < 0)
                _duration = $"{duration}us";
            else if (duration / 1000_000 < 0)
                _duration = $"{Math.Round(duration / 1000.0, 2)}ms";
            else
                _duration = $"{Math.Round(duration / 1000_000.0, 2)}s";
            _services = new List<string>();
            foreach (var item in _data)
            {              
                var serviceName = GetDictionaryValue(item, "service.name").ToString()!;
                if (!_services.Contains(serviceName))
                    _services.Add(serviceName);
            }
            
            _isLoading = false;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private List<DataTableHeader<Dictionary<string, object>>> _headers = new()
    {
        new("Service", item => ((Dictionary<string, object>)item["service"])["name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Endpoint", item => ((Dictionary<string, object>)item["transaction"])["name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Duration (ms)", item => ((Dictionary<string, object>)((Dictionary<string, object>)item["transaction"])["duration"])["us"])
        {
            Align = "start",
            Sortable = false
        },
        new("Timestamp", item => item["@timestamp"])
        {
            Align = "start",
            Sortable = false
        },
        new("EndTime", item => ((Dictionary<string, object>)item["transaction"])["name"])
        {
            Align = "start",
            Sortable = false
        }
    };

    private IEnumerable<object> _data = new List<object>();

    private string dsadasda(Dictionary<string, object> item)
    {
        return "";
    }

}
