// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record RangeQuery : Query<string>
{
    public string Match { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Step { get; set; }

    public override string Result { get; set; }
}
