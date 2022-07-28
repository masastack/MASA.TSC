// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record LableValuesQuery : Query<Dictionary<string, Dictionary<string, List<string>>>>
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public IEnumerable<string> Match { get; set; }

    public override Dictionary<string, Dictionary<string, List<string>>> Result { get; set; }
}
