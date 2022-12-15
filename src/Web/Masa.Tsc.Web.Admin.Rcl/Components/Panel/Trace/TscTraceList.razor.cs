// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public EventCallback<(int page, int size)> OnPaginationUpdate { get; set; }

    [Parameter]
    public PaginatedListBase<TraceResponseDto>? SearchResult { get; set; }

    [Parameter]
    public bool SearchLoading { get; set; }

    private TscTraceDetail? _tscTraceDetail;
    private IEnumerable<TraceResponseDto> _data = new List<TraceResponseDto>();
    private int _total = 0;
    private int _page = 1;
    private int _pageSize = 10;

    private List<DataTableHeader<TraceResponseDto>> _headers = new()
    {
        new("Service", item => item.Resource["service.name"])
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Endpoint", item => item.GetDispalyName())
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Duration (ms)", item => item.Duration)
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Timestamp", item => item.Timestamp)
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new DataTableHeader<TraceResponseDto>
        {
            Text = "Operate",
            Value = "Operate",
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        }
    };

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (SearchResult is not null)
        {
            _total = (int)SearchResult.Total;
            _data = SearchResult.Result;
        }
    }

    private async Task OpenAsync(TraceResponseDto item)
    {
        await _tscTraceDetail!.OpenAsync(item.TraceId);
    }

    private async Task HandleOnPaginationUpdate((int page, int pageSize) pagination)
    {
        await OnPaginationUpdate.InvokeAsync(pagination);
    }
}