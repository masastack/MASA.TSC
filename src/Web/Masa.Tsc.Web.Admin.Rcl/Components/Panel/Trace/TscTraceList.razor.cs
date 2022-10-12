// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public RequestTraceListDto Query { get; set; } = default!;

    private IEnumerable<TraceDto> _data = new List<TraceDto>();
    private int _total = 0;
    private MDataTable<TraceDto> _mDataTable = default!;
    private bool _isLoading = true;
    private string _selectTraceId = default!;    
    private List<DataTableHeader<TraceDto>> _headers = new()
    {
        new("Service", item =>item.Resource["service.name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Endpoint", item => item.IsHttp(out var traceHttpDto)? traceHttpDto.Target:item.Name)
        {
            Align = "start",
            Sortable = false
        },
        new("Duration (ms)", item =>item.GetDuration())
        {
            Align = "start",
            Sortable = false
        },
        new("Timestamp", item =>item.Timestamp)
        {
            Align = "start",
            Sortable = false
        },
        new DataTableHeader<TraceDto>
        {
            Text = "Operate",
            Value = "Operate",
            Align = "start",
            Sortable = false
        }
    };

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    private async Task OpenAsync(TraceDto item)
    {
        _selectTraceId = item.TraceId;
        OpenDialog();
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

    private async Task OnUpdateOptionsAsync(DataOptions options)
    {
        Query.Page = options.Page;
        Query.PageSize = options.ItemsPerPage;
        await QueryAsync(false);
    }

    private void OnItemSelect(TraceDto item, bool selected)
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
        _data = data.Items;
        _isLoading = false;
        if (isStateChange)
            StateHasChanged();
    }
}
