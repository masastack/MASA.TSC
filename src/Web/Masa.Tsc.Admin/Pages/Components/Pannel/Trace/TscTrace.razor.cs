// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTrace
{
    private TscTraceList _list;
    private TscTraceChart _chart;

    private RequestTraceListDto _query = new RequestTraceListDto();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task SearchAsync(string service, string instance, string endpoint, string traceId, DateTime start, DateTime end)
    {
        _query.Service = service;
        _query.Instance = instance;
        _query.Endpoint = endpoint;
        _query.TraceId = traceId;
        _query.Start = start;
        _query.End = end;
        await _list.QueryAsync();
        await _chart.LoadAsync();
        await Task.CompletedTask;
    }
}
