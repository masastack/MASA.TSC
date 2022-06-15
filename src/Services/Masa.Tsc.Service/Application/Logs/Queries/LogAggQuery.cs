﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application;

public record LogAggQuery : Query<IEnumerable<KeyValuePair<string, double>>>
{
    public IEnumerable<RequestLogFieldAggDto> FieldMaps { get; set; }

    public string Query { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public override IEnumerable<KeyValuePair<string, double>> Result { get; set; }
}
