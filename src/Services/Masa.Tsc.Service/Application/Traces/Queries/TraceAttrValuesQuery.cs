// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Traces;

public record TraceAttrValuesQuery(IEnumerable<KeyValuePair<string, object>> Query, string Name, string Keyword, DateTime? Start, DateTime? End, int Limit) : Query<IEnumerable<string>>
{
    public override IEnumerable<string> Result { get; set; }
}