// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record RangeValueQuery(string Match, DateTime Start, DateTime End) : Query<string>
{
    public override string Result { get; set; } = string.Empty;
}
