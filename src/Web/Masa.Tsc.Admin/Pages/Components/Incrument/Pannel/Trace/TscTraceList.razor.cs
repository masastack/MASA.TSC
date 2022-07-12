// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceList
{
    private List<DataTableHeader<TraceListDto>> _headers = new()
    {
        new()
        {
            Text = "Service",
            Align = "start",
            Sortable = false,
            Value = nameof(TraceListDto.Service)
        },
        new() { Text = "Endpoint", Value = nameof(TraceListDto.Endpoint) },
        new() { Text = "Duration (ms)", Value = nameof(TraceListDto.Duration) },
        new() { Text = "StatTime", Value = nameof(TraceListDto.Endpoint) },
        new() { Text = "EndTime", Value = nameof(TraceListDto.StartTime) }
    };    
}
