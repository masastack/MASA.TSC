// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record InstantQuery(string Match, DateTime Time) : Query<object>
{
    public override object Result { get; set; }
}
