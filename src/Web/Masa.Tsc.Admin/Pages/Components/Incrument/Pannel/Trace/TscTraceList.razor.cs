// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Web;

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceList : Shared.TscComponentBase
{
    [Parameter]
    public RequestTraceListDto Query { get; set; } = default!;
    private IEnumerable<Dictionary<string, object>> _data = new List<Dictionary<string, object>>();
    private int _total = 0;
    private MDataTable<Dictionary<string, object>> _mDataTable = default!;
    private bool _isLoading = true;
    private string _selectTraceId = default!;
    private bool _showDialog = false;
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
        new DataTableHeader<Dictionary<string, object>>
        {
            Text = "Operate",
            Value = "Operate",
            Align = "start",
            Sortable = false
        }
    };

    private async Task OpenAsync(Dictionary<string, object> item)
    {
        _selectTraceId = GetDictionaryValue(item, "trace.id").ToString()!;
        _showDialog = true;
        await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isLoading = false;
            await QueryAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    protected override Task OnParametersSetAsync()
    {
        return base.OnParametersSetAsync();
    }

    private async Task OnUpdateOptionsAsync(DataOptions options)
    {
        Query.Page = options.Page;
        Query.PageSize = options.ItemsPerPage;
        await QueryAsync(false);
    }

    private void OnItemSelect(Dictionary<string, object> item, bool selected)
    {
        OpenAsync(item).Wait();
    }

    public async Task QueryAsync(bool isStateChange = true)
    {
        if (isStateChange)
        {
            _mDataTable.Options.Page = 1;
            Query.Page = 1;
        }

        if (_isLoading) return;
        _isLoading = true;
        var data = await ApiCaller.TraceService.GetListAsync(Query);
        _total = (int)data.Total;
        _data = data.Items.Select(item => ((Dictionary<string, object>)((JsonElement)item).ToKeyValuePairs()!)).ToList();
        _isLoading = false;
        if (isStateChange)
            StateHasChanged();
    }
}
