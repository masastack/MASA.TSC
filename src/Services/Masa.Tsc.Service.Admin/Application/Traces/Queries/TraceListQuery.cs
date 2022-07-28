// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public record TraceListQuery(string Service, string Instance, string Endpoint, string TraceId, DateTime Start, DateTime End, int Page, int Size) : Query<PaginationDto<object>>
{
    public override PaginationDto<object> Result { get; set; }
}
