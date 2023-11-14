// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;
using System.Runtime.CompilerServices;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public EventCallback<(int page, int size, bool desc)> OnPaginationUpdate { get; set; }

    [Parameter]
    public PaginatedListBase<TraceResponseDto>? SearchResult { get; set; }

    [Parameter]
    public bool SearchLoading { get { return Loading; } set { Loading = value; } }

    [Parameter]
    public bool PageMode { get; set; }

    [Parameter]
    public int Page
    {
        get { return _page; }
        set { _page = value; }
    }

    [Parameter]
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = value; }
    }

    private TscTraceDetail? _tscTraceDetail;
    private IEnumerable<TraceResponseDto> _data = new List<TraceResponseDto>();
    private int _total = 0;
    private int _page = 1;
    private int _pageSize = 10;
    private bool _isDesc = true;
    private TraceResponseDto? CurrentTrace;
    private List<string> _sortBy = new() { "Timestamp" };
    private List<bool> _sortDesc = new() { true, false };

    private List<DataTableHeader<TraceResponseDto>> _headers => new()
    {
        new() { Text = I18n.T("Service"), Value = "Service", Sortable = false,Width=250},
        new() { Text = I18n.Trace("TraceID"), Value = "TraceId", Sortable = false },
        new() { Text = I18n.T("Endpoint"), Value = "Endpoint", Sortable = false},
        new() { Text = $"{ I18n.Trace("Duration")}", Value = "Duration", Sortable = false,Width=150},
        new() { Text = I18n.Trace("Timestamp"), Value = "Timestamp", Sortable = true,Width=200}
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
        await _tscTraceDetail!.OpenAsync(item.TraceId, item.SpanId);
    }

    private static string FormatDuration(double duration)
    {
        if (duration < 1)
            return "< 1ms";

        var ms = (long)Math.Round(duration, 0);

        if (ms < 1000)
        {
            return $"{ms}ms";
        }

        if (ms < 60000)
        {
            return $"{(ms / 1000d):F}s";
        }

        return $"{(ms / 60000d):F}m";
    }

    private async Task HandleOnPaginationUpdate((int page, int pageSize) pagination)
    {
        await OnPaginationUpdate.InvokeAsync((pagination.page, pagination.pageSize, _isDesc));
    }
    private async Task OnOptionsUpdate(DataOptions options)
    {
        _isDesc = options.SortDesc.FirstOrDefault();
        //await OnPaginationUpdate.InvokeAsync((1, _pageSize, _isDesc));
    }

    protected override bool IsSubscribeTimeZoneChange => true;

    protected override async Task OnTimeZoneInfoChanged(TimeZoneInfo timeZoneInfo)
    {
        await base.OnTimeZoneInfoChanged(timeZoneInfo);
        StateHasChanged();
    }

    private string GetDisplayName(TraceResponseDto dto)
    {
        if (dto.Attributes.ContainsKey("db.system"))
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
            defaultInterpolatedStringHandler.AppendFormatted<object>(dto.Attributes.ContainsKey("peer.service") ? dto.Attributes["peer.service"] : "");
            defaultInterpolatedStringHandler.AppendFormatted(dto.Attributes["db.system"]);
            defaultInterpolatedStringHandler.AppendFormatted(dto.Attributes["db.name"]);
            return defaultInterpolatedStringHandler.ToStringAndClear();
        }
        else if (dto.Attributes.ContainsKey("http.method"))
        {
            if (dto.Kind == "SPAN_KIND_CLIENT")
            {
                return dto.Attributes["http.url"].ToString();
            }
            else
                return dto.Attributes["http.target"].ToString();
        }

        return dto.Name;
    }
}
