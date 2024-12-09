// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record ValuesQuery(string Layer, string Service, string Instance, string Endpint, MetricValueTypes Type) : Query<List<string>>
{
    public override List<string> Result { get; set; }
}
