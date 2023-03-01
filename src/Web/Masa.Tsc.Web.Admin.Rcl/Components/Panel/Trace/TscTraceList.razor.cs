// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public EventCallback<(int page, int size, bool desc)> OnPaginationUpdate { get; set; }

    [Parameter]
    public PaginatedListBase<TraceResponseDto>? SearchResult { get; set; }

    [Parameter]
    public bool SearchLoading { get; set; }

    [Parameter]
    public bool PageMode { get; set; }

    private TscTraceDetail? _tscTraceDetail;
    private IEnumerable<TraceResponseDto> _data = new List<TraceResponseDto>();
    private int _total = 0;
    private int _page = 1;
    private int _pageSize = 10;
    private bool _isDesc = true;
    private TraceResponseDto? CurrentTrace;
    private List<string> _sortBy = new List<string> {
        "Timestamp"
    };
    private List<bool> _sortDesc = new List<bool> { true, false };

    private List<DataTableHeader<TraceResponseDto>> _headers => new()
    {
        new() { Text = T("Service"), Value = "Service", Sortable = false },
        new() { Text = T("TraceId"), Value = "TraceId", Sortable = false },
        new() { Text = T("Endpoint"), Value = "Endpoint", Sortable = false },
        new() { Text = T("Duration (ms)"), Value = "Duration", Sortable = false},
        new() { Text = T("Timestamp"), Value = "Timestamp", Sortable = true}
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
        if (CurrentTrace == null || CurrentTrace.SpanId == item.SpanId)
        {
            CurrentTrace = item;
        }
        await _tscTraceDetail!.OpenAsync(item.TraceId);
    }

    private async Task HandleOnPaginationUpdate((int page, int pageSize) pagination)
    {
        await OnPaginationUpdate.InvokeAsync((pagination.page, pagination.pageSize, _isDesc));
    }
    private async Task OnOptionsUpdate(DataOptions options)
    {
        _isDesc = options.SortDesc.FirstOrDefault();
        await OnPaginationUpdate.InvokeAsync((1, _pageSize, _isDesc));
    }

    public void SetTimeZoneInfo(TimeZoneInfo timeZoneInfo)
    {
        CurrentTimeZone = timeZoneInfo;
    }
}