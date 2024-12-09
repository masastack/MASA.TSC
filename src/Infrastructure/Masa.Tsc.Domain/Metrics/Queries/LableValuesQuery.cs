// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record LableValuesQuery(IEnumerable<string> Match, DateTime Start, DateTime End) : Query<Dictionary<string, Dictionary<string, List<string>>>>
{
    public override Dictionary<string, Dictionary<string, List<string>>> Result { get; set; }
}
