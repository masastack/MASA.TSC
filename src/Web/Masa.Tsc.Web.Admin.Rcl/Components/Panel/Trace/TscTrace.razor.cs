// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Models;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTrace
{
    private TscTraceChart _chart;

    private PaginatedListBase<TraceResponseDto> _queryResult;

    private string _service;
    private string _instance;
    private string _endpoint;
    private string _traceId;
    private DateTime _startDateTime;
    private DateTime _endDateTime;

    private int _page = 1;
    private int _pageSize = 10;

    private async Task Search((DateTime start, DateTime end) dateTimes)
    {
        _startDateTime = dateTimes.start;
        _endDateTime = dateTimes.end;

        await SearchAsync();
    }

    private async Task Search((string service, string instance, string endpoint, string traceId) query)
    {
        _service = query.service;
        _instance = query.instance;
        _endpoint = query.endpoint;
        _traceId = query.traceId;

        await SearchAsync();
    }

    private async Task Search((int page, int size) pagination)
    {
        _page = pagination.page;
        _pageSize = pagination.size;

        await SearchAsync();
    }

    private async Task SearchAsync()
    {
        RequestTraceListDto query = new()
        {
            Service = _service,
            Instance = _instance,
            Endpoint = _endpoint,
            TraceId = _traceId,
            Start = _startDateTime,
            End = _endDateTime,
            Page = _page,
            PageSize = _pageSize
        };

        _queryResult = await ApiCaller.TraceService.GetListAsync(query);
    }
}
