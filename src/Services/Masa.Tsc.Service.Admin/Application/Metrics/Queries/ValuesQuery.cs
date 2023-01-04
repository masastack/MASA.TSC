// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Metrics.Queries;

public record ValuesQuery(string Service,MetricValueTypes Type) : Query<List<string>>
{
    public override List<string> Result { get; set; }
}
