// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record InstantQuery:Query<object>
{
    public string Match { get; set; }

    public DateTime? Time { get; set; }

    public override object Result { get; set; }
}
