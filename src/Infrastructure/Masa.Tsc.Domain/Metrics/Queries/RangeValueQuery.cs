// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record RangeValueQuery(string Match, DateTime Start, DateTime End) : Query<string>
{
    public override string Result { get; set; } = string.Empty;
}
