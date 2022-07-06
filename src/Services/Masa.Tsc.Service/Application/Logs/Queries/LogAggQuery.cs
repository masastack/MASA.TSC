// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogAggQuery : Query<IEnumerable<KeyValuePair<string, string>>>
{
    public IEnumerable<RequestLogFieldAggDto> FieldMaps { get; set; }

    public string Query { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public override IEnumerable<KeyValuePair<string, string>> Result { get; set; }
}
