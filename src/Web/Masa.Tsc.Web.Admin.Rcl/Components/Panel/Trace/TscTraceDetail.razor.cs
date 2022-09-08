// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceDetail
{
    [Parameter]
    public string TraceId { get { return _traceId; } set { _traceId = value; _tabIndex = "attr"; } }

    private StringNumber _tabIndex = "attr";
    private IEnumerable<object> _items = new List<object>();
    private bool _isLoading = true;
    private string _traceId = default!;
    private TraceDetailModel _selectItem;
    private TraceOverviewModel _overView = new TraceOverviewModel();
    private List<TraceOverviewServiceModel> _services = new List<TraceOverviewServiceModel>();    

    private async Task OverviewCompeleteAysnc(TraceOverviewModel model)
    {
        _overView = model;
        _services = model.Services;
        await Task.CompletedTask;
    }

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        var data = await ApiCaller.TraceService.GetAsync(TraceId);
        if (data == null)
            return;

        _items = data.Select(item => (object)((JsonElement)item).ToKeyValuePairs()!);
        await ChangeRecordAsync(_items.FirstOrDefault()!);
        await base.OnParametersSetAsync();
        _isLoading = false;
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

    private async Task ChangeRecordAsync(object item)
    {
        _selectItem = new TraceDetailModel((Dictionary<string, object>)item);
        StateHasChanged();
        await Task.CompletedTask;
    }
}
