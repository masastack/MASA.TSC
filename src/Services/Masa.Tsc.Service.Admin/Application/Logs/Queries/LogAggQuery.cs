// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogAggQuery(IEnumerable<RequestFieldAggregationDto> FieldMaps, string Query, DateTime Start, DateTime End,string Interval) : Query<IEnumerable<KeyValuePair<string, string>>>
{
    public override IEnumerable<KeyValuePair<string, string>> Result { get; set; }
}
