// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics;

public record MetricQuery : Query<IEnumerable<string>>
{
    public IEnumerable<string> Match { get; set; }

    public override IEnumerable<string> Result { get; set; }
}
