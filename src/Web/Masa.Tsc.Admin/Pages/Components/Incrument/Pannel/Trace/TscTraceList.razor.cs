// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceList
{
    private List<DataTableHeader<RequestTraceListDto>> _headers = new()
    {
        new()
        {
            Text = "Service",
            Align = "start",
            Sortable = false,
            Value = nameof(RequestTraceListDto.Service)
        },
        new() { Text = "Endpoint", Value = nameof(RequestTraceListDto.Endpoint) },
        new() { Text = "Duration (ms)", Value = "Duration" },
        new() { Text = "StatTime", Value = nameof(RequestTraceListDto.Endpoint) },
        new() { Text = "EndTime", Value = nameof(RequestTraceListDto.Start) }
    };
}
